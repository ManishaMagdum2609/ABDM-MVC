using Asp.netWebAPP.Core.Application.DTO_s;
using MediatR;

namespace Asp.netWebAPP.Core.Application.M2.Commands
{
    public class LinkCareContextCommand : IRequest<CareContextLinkResponseDTO>
    {
        public CareContextLinkRequestDTO Request { get; set; }
    }
}
