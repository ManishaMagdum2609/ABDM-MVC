using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Core.Application.M2.Commands;
using Asp.netWebAPP.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Asp.netWebAPP.Infrastructure.Services
{
    public class BridgeService : IBridgeService
    {
        private readonly HttpClient _httpClient;
        private readonly AbdmDbContext _dbContext;
        private readonly IAbhaAuthService _authService;
        private readonly ILogger<BridgeService> _logger;

        public BridgeService(HttpClient httpClient,
                             AbdmDbContext dbContext,
                             IAbhaAuthService authService,
                             ILogger<BridgeService> logger)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _authService = authService;
            _logger = logger;
        }

        public async Task<RegisterBridgeServiceResponseDTO> RegisterBridgeServiceAsync(RegisterBridgeServiceCommand command)
        {
            var config = await _authService.GetAbdmConfigAsync();
            var token = await _authService.GetAccessTokenAsync();

            var url = config.registerBridgeServiceUrl; // value from DB "bridgeservice" config

            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Bridge service registration failed. StatusCode: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);

                return new RegisterBridgeServiceResponseDTO
                {
                    Success = false,
                    Status = (int)response.StatusCode,
                    Message = responseContent
                };
            }

            _logger.LogInformation("Bridge service registration success: {Response}", responseContent);

            return new RegisterBridgeServiceResponseDTO
            {
                Success = true,
                Status = (int)response.StatusCode,
                Data = responseContent
            };
        }

       
    }


}
