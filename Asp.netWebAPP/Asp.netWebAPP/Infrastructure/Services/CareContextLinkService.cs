using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

namespace Asp.netWebAPP.Infrastructure.Services
{
    public class CareContextLinkService : ICareContextLinkService
    {
        private readonly HttpClient _httpClient;
        private readonly AbdmDbContext _dbContext;
        private readonly IAbhaAuthService _authService;
        private readonly DanpheDbContext _danpheDbContext;
        private readonly IDataProtectionProvider _dataProtectionProvider;   


        public CareContextLinkService(HttpClient httpClient, AbdmDbContext dbContext, IAbhaAuthService authService,
            DanpheDbContext danpheDbContext, IDataProtectionProvider dataProtectionProvider)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _authService = authService;
            _danpheDbContext = danpheDbContext;
            _dataProtectionProvider = dataProtectionProvider;
        }

        // public DanpheDbContext DanpheDbContext { get; }

        public async Task<CareContextLinkResponseDTO> LinkCareContextAsync(CareContextLinkRequestDTO request)
        {

            var config = await _authService.GetAbdmConfigAsync();

            var accessToken = await _authService.GetAccessTokenAsync();

            // 🔹 STEP 1: Check DB if link token exists for ABHA Number


            var existingPatient = await _danpheDbContext.Patient
                .FirstOrDefaultAsync(p => p.EHRNumber == request.AbhaNumber);
            if (existingPatient == null)
            {
                // handle patient not found
                throw new Exception("Patient not found.");
            }


            string fullName = string.Join(" ",
             new[] { existingPatient.FirstName, existingPatient.MiddleName, existingPatient.LastName }
                    .Where(n => !string.IsNullOrWhiteSpace(n))); 

            string genderShort = existingPatient.Gender?.Trim().ToLower() switch
            {
                "male" => "M",
                "female" => "F",
                _ => "U"
            };

            string yearOfBirth = existingPatient.DateOfBirth.Year.ToString();
            var protector = _dataProtectionProvider.CreateProtector("LinkToken");

            string linkToken;
            if (!string.IsNullOrWhiteSpace(existingPatient.EncryptedLinkToken) &&
               existingPatient.LinkTokenExpiry.HasValue &&
               existingPatient.LinkTokenExpiry.Value > DateTime.UtcNow)
            {
                // Use existing token
                linkToken = protector.Unprotect(existingPatient.EncryptedLinkToken);
            }
            //if (existingToken != null && existingToken.Expiry > DateTime.UtcNow)
            //{
            //    // 🔹 Use existing link token
            //    linkToken = existingToken.Token;
            //}
            else
            {
                // 🔹 STEP 2: Generate new link token
                var generateRequest = new GenerateLinkTokenRequestDTO
                {
                    AbhaNumber = request.AbhaNumber,
                    AbhaAddress = request.AbhaAddress,
                    Name = fullName,         
                    Gender = genderShort,     
                    YearOfBirth = yearOfBirth 
                };

                var newTokenResponse = await GenerateLinkTokenAsync(generateRequest);
                linkToken = newTokenResponse.LinkToken;

                // Encrypt and save token in Patient table
                existingPatient.EncryptedLinkToken = protector.Protect(linkToken);
                existingPatient.LinkTokenExpiry = DateTime.UtcNow.AddMonths(6); // 6 months validity

                _danpheDbContext.Patient.Update(existingPatient);
                await _danpheDbContext.SaveChangesAsync();
            }

            // 🔹 STEP 3: Call ABDM API with valid link token
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, config.careContextLinkUrl);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpRequest.Headers.Add("REQUEST-ID", Guid.NewGuid().ToString());
            httpRequest.Headers.Add("TIMESTAMP", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            httpRequest.Headers.Add("X-CM-ID", "sbx");
            httpRequest.Headers.Add("X-HIP-ID", config.hipId);
            httpRequest.Headers.Add("X-LINK-TOKEN", linkToken);

            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            return new CareContextLinkResponseDTO
            {
                TransactionId = Guid.NewGuid().ToString(),
                Status = response.IsSuccessStatusCode ? "Success" : "Failed",
                Message = content
            };
        }

        private async Task<GenerateLinkTokenResponseDTO> GenerateLinkTokenAsync(GenerateLinkTokenRequestDTO request)
        {
            //var config = await _dbContext.AbdmCore_Parameters.FirstAsync(x => x.ParameterGroupName == "ABDM");
            //var abdmConfig = JsonSerializer.Deserialize<AbdmConfigDTO>(config.ParameterValue);

            var config = await _authService.GetAbdmConfigAsync();
            var token = await _authService.GetAccessTokenAsync();

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, config.generateLinkTokenUrl);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Headers.Add("REQUEST-ID", Guid.NewGuid().ToString());
            httpRequest.Headers.Add("TIMESTAMP", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            httpRequest.Headers.Add("X-CM-ID", "sbx");
            httpRequest.Headers.Add("X-HIP-ID", config.hipId);

            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            return new GenerateLinkTokenResponseDTO
            {
                LinkToken = "Extract_from_response_JSON",
                Expiry = DateTime.UtcNow.AddMonths(6),
                Status = response.IsSuccessStatusCode ? "Generated" : "Failed"
            };
        }
    }
}
