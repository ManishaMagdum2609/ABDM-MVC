using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.M2.Commands;

namespace Asp.netWebAPP.Core.Application.Interface
{
    public interface IBridgeService
    {
        Task<RegisterBridgeServiceResponseDTO> RegisterBridgeServiceAsync(RegisterBridgeServiceCommand command);
    }

}
