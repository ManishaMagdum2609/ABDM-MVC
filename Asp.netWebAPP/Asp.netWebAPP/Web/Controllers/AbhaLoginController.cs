using Microsoft.AspNetCore.Mvc;
using Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers;
using Asp.netWebAPP.Core.Application.ABHA.Commands;
using Asp.netWebAPP.Core.Application.ABHA.Queries.Handler;
using Asp.netWebAPP.Core.Application.ABHA.Queries;
using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Domain.Model;

namespace Backend.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbhaLoginController : ControllerBase
    {
        private readonly SearchAbhaHandler _searchHandler;
        private readonly RequestOtpLoginHandler _otpHandler;
        private readonly VerifyOtpHandler _verifyHandler;

        public AbhaLoginController(
            SearchAbhaHandler searchHandler,
            RequestOtpLoginHandler otpHandler,
            VerifyOtpHandler verifyHandler)
        {
            _searchHandler = searchHandler;
            _otpHandler = otpHandler;
            _verifyHandler = verifyHandler;
        }

        [HttpPost("search-abha")]
        public async Task<ActionResult<List<AbhaAccount>>> SearchAbha([FromBody] SearchAbhaRequest request)
        {
            var result = await _searchHandler.Handle(new SearchAbhaQuery(request.Mobile));
            return Ok(result);
        }

        [HttpPost("request-otp-login")]
        public async Task<ActionResult<OtpResponse>> RequestOtpLogin([FromBody] RequestOtpLoginCommand command)
        {
            var result = await _otpHandler.Handle(command);
            return Ok(result);
        }

        [HttpPost("verify-abha-login")]
        public async Task<ActionResult<VerifyOtpResponse>> VerifyAbhaLogin([FromBody] VerifyOtpCommand command)
        {
            var result = await _verifyHandler.Handle(command);
            return Ok(result);
        }
    }
}
