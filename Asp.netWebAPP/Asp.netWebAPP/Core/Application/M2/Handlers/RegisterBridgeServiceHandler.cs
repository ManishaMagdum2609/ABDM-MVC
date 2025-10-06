using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Core.Application.M2.Commands;
using MediatR;

namespace Asp.netWebAPP.Core.Application.M2.Handlers
{
    public class RegisterBridgeServiceHandler : IRequestHandler<RegisterBridgeServiceCommand, RegisterBridgeServiceResponseDTO>
    {
        private readonly IBridgeService _bridgeService;

        public RegisterBridgeServiceHandler(IBridgeService bridgeService)
        {
            _bridgeService = bridgeService;
        }

        public async Task<RegisterBridgeServiceResponseDTO> Handle(RegisterBridgeServiceCommand request, CancellationToken cancellationToken)
        {
            return await _bridgeService.RegisterBridgeServiceAsync(request);
        }
    }

}
