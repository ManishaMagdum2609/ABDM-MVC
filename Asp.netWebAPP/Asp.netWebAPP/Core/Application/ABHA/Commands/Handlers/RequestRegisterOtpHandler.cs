using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;

namespace Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers
{
    public class RequestRegisterOtpHandler
    {
        private readonly IAbhaRegistrationService _service;

        public RequestRegisterOtpHandler(IAbhaRegistrationService service)
        {
            _service = service;
        }

        public async Task<OtpResponse> Handle(RequestRegisterOtpCommand command)
        {
            return await _service.RequestRegisterOtpAsync(command.AadhaarNumber);
        }
    }
}
