namespace Asp.netWebAPP.Core.Application.Interface
{
    public interface IAbhaAuthService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetPublicKeyAsync();
    }
}
