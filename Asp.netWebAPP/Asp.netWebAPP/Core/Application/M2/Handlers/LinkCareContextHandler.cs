using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Core.Application.M2.Commands;
using MediatR;

namespace Asp.netWebAPP.Core.Application.M2.Handlers
{
    public class LinkCareContextHandler : IRequestHandler<LinkCareContextCommand, CareContextLinkResponseDTO>
    {
        private readonly ICareContextLinkService _service;

        public LinkCareContextHandler(ICareContextLinkService service)
        {
            _service = service;
        }

        public async Task<CareContextLinkResponseDTO> Handle(LinkCareContextCommand request, CancellationToken cancellationToken)
        {
            return await _service.LinkCareContextAsync(request.Request);
        }
    }
}
