using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;

namespace Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers
{
    public class VerifyRegisterOtpHandler
    {
        private readonly IAbhaRegistrationService _abhaService;

        public VerifyRegisterOtpHandler(IAbhaRegistrationService abhaService)
        {
            _abhaService = abhaService;
        }

        public async Task<VerifyRegisterOtpResponse> Handle(VerifyRegisterOtpCommand command)
        {
            return await _abhaService.VerifyAbhaRegistrationAsync(
                command.TxnId,
                command.Otp,
                command.Mobile
            );
        }
    }
}
