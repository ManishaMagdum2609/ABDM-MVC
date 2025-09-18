using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;

namespace Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers
{
    public class VerifyOtpHandler
    {
        private readonly IAbhaLoginService _service;

        public VerifyOtpHandler(IAbhaLoginService service)
        {
            _service = service;
        }

        public async Task<VerifyOtpResponse> Handle(VerifyOtpCommand command)
        {
            return await _service.VerifyAbhaLoginAsync(command.TxnId, command.Otp);
        }
    }
}
