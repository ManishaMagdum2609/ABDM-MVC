using Asp.netWebAPP.Core.Application.DTO_s;

namespace Asp.netWebAPP.Core.Application.Interface
{
    public interface ICareContextLinkService
    {
        Task<CareContextLinkResponseDTO> LinkCareContextAsync(CareContextLinkRequestDTO request);
    }
}
