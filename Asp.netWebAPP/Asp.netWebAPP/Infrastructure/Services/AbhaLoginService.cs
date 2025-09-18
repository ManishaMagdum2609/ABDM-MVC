using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Core.Domain.Model;
using Asp.netWebAPP.Core.Domain.Value_Objects;
using Asp.netWebAPP.Infrastructure.Data;
using Asp.netWebAPP.Infrastructure.Security;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using VerifyOtpResponse = Asp.netWebAPP.Core.Application.DTO_s.VerifyOtpResponse;
using Microsoft.EntityFrameworkCore;

namespace Asp.netWebAPP.Infrastructure.Services
{
    public class Abhalogin_Service : IAbhaLoginService
    {
        private readonly AbdmDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public Abhalogin_Service(AbdmDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }

        private async Task<Dictionary<string, string>> GetAbdmSettingsAsync()
        {
            var row = await _dbContext.AbdmCore_Parameters
                .FirstOrDefaultAsync(p => p.ParameterGroupName == "ABDM");

            if (row == null)
                throw new Exception("ABDM_Config row not found in Core_Parameters");
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(row.ParameterValue);

            return dict;
        }

        private async Task<string> GetAccessTokenAsync(Dictionary<string, string> settings)
        {
            if (!settings.TryGetValue("clientId", out var clientId) ||
                !settings.TryGetValue("clientSecret", out var clientSecret) ||
                !settings.TryGetValue("tokenUrl", out var tokenUrl))
            {
                throw new Exception("Missing ABDM config values from database.");
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            _httpClient.DefaultRequestHeaders.Add("X-CM-ID", "sbx");

            var tokenRequest = new
            {
                clientId,
                clientSecret,
                grantType = "client_credentials"
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(tokenRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(tokenUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenObj = JsonSerializer.Deserialize<JsonElement>(json);

            return tokenObj.GetProperty("accessToken").GetString();
        }

        private async Task<string> GetPublicKeyAsync(string token, Dictionary<string, string> settings)
        {
            if (!settings.TryGetValue("publicKeyUrl", out var publicKeyUrl))
                throw new Exception("ABDM publicKeyUrl is missing in Core_Parameters");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var publicKeyResponse = await _httpClient.GetAsync(publicKeyUrl);
            publicKeyResponse.EnsureSuccessStatusCode();

            return await publicKeyResponse.Content.ReadAsStringAsync();
        }
        private string EncryptAadhaar(string aadhaarNumber, string publicKeyResponse)
        {
            if (string.IsNullOrWhiteSpace(aadhaarNumber))
                throw new ArgumentException("Aadhaar number cannot be empty.");

            if (string.IsNullOrWhiteSpace(publicKeyResponse))
                throw new ArgumentException("Public key response cannot be empty.");

            string publicKeyPem;
            if (publicKeyResponse.TrimStart().StartsWith("{"))
            {
                var jsonDoc = JsonDocument.Parse(publicKeyResponse);
                var base64Key = jsonDoc.RootElement.GetProperty("publicKey").GetString();
                publicKeyPem = "-----BEGIN PUBLIC KEY-----\n" +
                               base64Key +
                               "\n-----END PUBLIC KEY-----";
            }
            else
            {
                publicKeyPem = publicKeyResponse;
            }

            return AadhaarEncryptor.EncryptWithPublicKeyString(aadhaarNumber, publicKeyPem);
        }

        public async Task<List<AbhaAccount>> SearchAbhaAsync(string mobile)
        {
            var settings = await GetAbdmSettingsAsync();
            var token = await GetAccessTokenAsync(settings);

            if (!settings.TryGetValue("searchAbhaUrl", out var searchUrl))
                throw new Exception("ABDM searchAbhaUrl is missing in Core_Parameters");

            var publicKey = await GetPublicKeyAsync(token, settings);

            var encryptedMobile = AadhaarEncryptor.EncryptWithPublicKeyString(mobile, publicKey);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            _httpClient.DefaultRequestHeaders.Add("X-CM-ID", "sbx");

            var body = new
            {
                scope = new[] { "search-abha" },
                mobile = encryptedMobile
            };

            var response = await _httpClient.PostAsJsonAsync(searchUrl, body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"ABDM search failed: {response.StatusCode} - {json}");

            return JsonSerializer.Deserialize<List<AbhaAccount>>(json) ?? new List<AbhaAccount>();
        }

        public async Task<OtpResponse> RequestOtpLoginAsync(int index, string TxnId)
        {
            var settings = await GetAbdmSettingsAsync();
            var token = await GetAccessTokenAsync(settings);

            if (!settings.TryGetValue("abhaLoginOTPRequestUrl", out var abhaUrl))
                throw new Exception("ABDM abhaOTPrequestUrl is missing in Core_Parameters");

            var publicKey = await GetPublicKeyAsync(token, settings);

            var encryptedIndex = AadhaarEncryptor.EncryptWithPublicKeyString(index.ToString(), publicKey);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("BENEFIT_NAME", "benefit-name");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            _httpClient.DefaultRequestHeaders.Add("X-CM-ID", "sbx");

            var body = new
            {
                scope = new[] { "abha-login", "search-abha", "mobile-verify" },
                loginHint = "index",
                loginId = encryptedIndex,
                otpSystem = "abdm",
                txnId = TxnId
            };

            var response = await _httpClient.PostAsJsonAsync(abhaUrl, body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"ABDM OTP request failed: {response.StatusCode} - {json}");

            return JsonSerializer.Deserialize<OtpResponse>(json);
        }

        public async Task<VerifyOtpResponse> VerifyAbhaLoginAsync(string txnId, string otp)
        {
            var settings = await GetAbdmSettingsAsync();
            var token = await GetAccessTokenAsync(settings);

            if (!settings.TryGetValue("abhaLoginUrl", out var verifyUrl))
                throw new Exception("ABDM abhaLoginUrl is missing in Core_Parameters");

            var publicKey = await GetPublicKeyAsync(token, settings);
            var encryptedOtp = AadhaarEncryptor.EncryptWithPublicKeyString(otp, publicKey);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            _httpClient.DefaultRequestHeaders.Add("X-CM-ID", "sbx");

            var body = new
            {
                scope = new[] { "abha-login", "mobile-verify" },
                authData = new
                {
                    authMethods = new[] { "otp" },
                    otp = new
                    {
                        txnId = txnId,
                        otpValue = encryptedOtp
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(verifyUrl, body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"ABDM Verify OTP failed: {response.StatusCode} - {json}");

            // ✅ Deserialize original ABDM payload
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var result = new VerifyOtpResponse
            {
                TxnId = txnId,
                Accounts = new List<AbhaAccountDTO>()
            };

            if (root.TryGetProperty("accounts", out var accounts))
            {
                foreach (var acc in accounts.EnumerateArray())
                {
                    result.Accounts.Add(new AbhaAccountDTO
                    {
                        ABHANumber = acc.GetProperty("ABHANumber").GetString(),
                        PreferredAbhaAddress = acc.GetProperty("preferredAbhaAddress").GetString(),
                        Name = acc.GetProperty("name").GetString()
                    });
                }
            }

            return result;
        }

    }
}
