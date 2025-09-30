using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Asp.netWebAPP.Infrastructure.Services
{
    public class AbhaAuthService : IAbhaAuthService
    {
        private readonly AbdmDbContext _dbContext;
        private readonly HttpClient _httpClient;
        public AbhaAuthService(AbdmDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
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
        public async Task<string> GetAccessTokenAsync()
        {
            var config = await GetAbdmConfigAsync();

            if (string.IsNullOrEmpty(config.clientId) || string.IsNullOrEmpty(
                config.clientSecret) || string.IsNullOrEmpty(config.tokenUrl))
                throw new Exception("Missing ABDM config values.");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            _httpClient.DefaultRequestHeaders.Add("X-CM-ID", "sbx");

            var tokenRequest = new
            {
                clientId = config.clientId,
                clientSecret = config.clientSecret,
                grantType = "client_credentials"
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(tokenRequest),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(config.tokenUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenObj = JsonSerializer.Deserialize<JsonElement>(json);

            return tokenObj.GetProperty("accessToken").GetString();
        }


        public async Task<string> GetPublicKeyAsync()
        {
            var config = await GetAbdmConfigAsync();
            var token = await GetAccessTokenAsync();

            if (string.IsNullOrEmpty(config.publicKeyUrl))
                throw new Exception("ABDM publicKeyUrl is missing.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", token);

            var publicKeyResponse = await _httpClient.GetAsync(config.publicKeyUrl);
            publicKeyResponse.EnsureSuccessStatusCode();

            return await publicKeyResponse.Content.ReadAsStringAsync();
        }
    }
}
