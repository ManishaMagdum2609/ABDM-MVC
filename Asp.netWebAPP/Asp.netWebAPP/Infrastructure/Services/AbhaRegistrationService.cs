using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Asp.netWebAPP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Asp.netWebAPP.Infrastructure.Data;
using Asp.netWebAPP.Core.Domain.Value_Objects;

namespace Asp.netWebAPP.Infrastructure.Services
{
    public class AbhaRegistrationService : IAbhaRegistrationService
    {
        private readonly HttpClient _httpClient;
        private readonly AbdmDbContext _dbContext;
        private readonly AbdmConfigDTO _config;
        private readonly IAbhaAuthService _authService;
        public AbhaRegistrationService(AbdmDbContext dbContext,
                                       IAbhaAuthService authService,
                                       HttpClient httpClient,
                                       IAbhaLoginService loginService)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _authService = authService;
        }
        private async Task<AbdmConfigDTO> GetAbdmConfigAsync()
        {
            var row = await _dbContext.AbdmCore_Parameters
                .FirstOrDefaultAsync(p => p.ParameterGroupName == "ABDM");

            if (row == null)
                throw new Exception("ABDM_Config row not found in Core_Parameters");

            return JsonSerializer.Deserialize<AbdmConfigDTO>(row.ParameterValue)
                   ?? throw new Exception("Failed to deserialize ABDM config.");
        }
        public async Task<OtpResponse> RequestRegisterOtpAsync(string aadhaarNumber)
        {
            var config = await GetAbdmConfigAsync();
            var accessToken = await _authService.GetAccessTokenAsync();
            string publicKeyResponse = await _authService.GetPublicKeyAsync();
            string encryptedAadhaar = Encryptor.EncryptWithPublicKeyString(
                aadhaarNumber, publicKeyResponse);


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("X-CM-ID", "sbx");
            _httpClient.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));

            var payload = new
            {
                txnId = "",
                scope = new[] { "abha-enrol" },
                loginHint = "aadhaar",
                loginId = encryptedAadhaar,
                otpSystem = "aadhaar"
            };
            var response = await _httpClient.PostAsJsonAsync(config.abhaOTPrequestUrl, payload);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Error sending OTP: {json}");

            return JsonSerializer.Deserialize<OtpResponse>(json);
        }

        public async Task<VerifyRegisterOtpResponse> VerifyAbhaRegistrationAsync(string txnId, 
            string otp, 
            string mobile)
        {
            if (string.IsNullOrWhiteSpace(txnId) || string.IsNullOrWhiteSpace(otp) || string.IsNullOrWhiteSpace(mobile))
                throw new ApplicationException("TxnId, OTP, and Mobile are required.");
            var config = await GetAbdmConfigAsync();
            var accessToken = await _authService.GetAccessTokenAsync();
            var publicKey = await _authService.GetPublicKeyAsync();
            var encryptedOtp = Encryptor.EncryptWithPublicKeyString(otp, publicKey);
            var requestBody = new
            {
                authData = new
                {
                    authMethods = new[] { "otp" },
                    otp = new
                    {
                        txnId,
                        otpValue = encryptedOtp,
                        mobile
                    }
                },
                consent = new
                {
                    code = "abha-enrollment",
                    version = "1.4"
                }
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );
            var request = new HttpRequestMessage(HttpMethod.Post, config.abhaCreationUrl)
            {
                Content = requestContent
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("X-CM-ID", "sbx");
            request.Headers.Add("REQUEST-ID", Guid.NewGuid().ToString());
            request.Headers.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("ABHA API Response: " + responseString);

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"ABHA API returned {response.StatusCode}: {responseString}");
            var result = JsonSerializer.Deserialize<VerifyRegisterOtpResponse>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (result?.ABHAProfile != null)
            {
                result.ABHAProfile.Mobile = mobile;
                result.ABHAProfile.AbhaNumber = result.ABHAProfile.PreferredAddress?.Split('@')[0];
            }

            return result;
        }

    }
}
