using Asp.netWebAPP.Core.Application.DTO_s;

namespace Asp.netWebAPP.Core.Application.Interface
{
    public interface IAbhaAuthService
    {
        Task<AbdmConfigDTO> GetAbdmConfigAsync(); 
        Task<string> GetAccessTokenAsync();
        Task<string> GetPublicKeyAsync();
    }
}
