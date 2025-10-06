using Asp.netWebAPP.Core.Application.DTO_s;
using MediatR;

namespace Asp.netWebAPP.Core.Application.M2.Commands
{
    public class RegisterBridgeServiceCommand : IRequest<RegisterBridgeServiceResponseDTO>
    {
        public string FacilityId { get; set; }
        public string FacilityName { get; set; }
        public List<HRPRequestDTO> HRP { get; set; }
    }

    public class HRPRequestDTO
    {
        public string BridgeId { get; set; }
        public string HipName { get; set; }
        public string Type { get; set; } = "HIP";
        public bool Active { get; set; }
    }
}
