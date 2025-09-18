using Asp.netWebAPP.Core.Application.ABHA.Queries.Handler;
using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.Interface;

namespace Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers
{
    public class RequestOtpLoginHandler
    {
        
            private readonly IAbhaLoginService _service;

            public RequestOtpLoginHandler(IAbhaLoginService service)
            {
                _service = service;
            }

            public async Task<OtpResponse> Handle(RequestOtpLoginCommand command)
            {
                return await _service.RequestOtpLoginAsync(command.Index, command.TxnId);
            }
        
    }
}
