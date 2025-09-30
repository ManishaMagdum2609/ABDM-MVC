using Asp.netWebAPP.Core.Application.ABHA.Commands;
using Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers;
using Asp.netWebAPP.Core.Application.ABHA.Queries.Handler;
using Asp.netWebAPP.Core.Application.DTO_s;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netWebAPP.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbhaRegisterController : ControllerBase
    {
        private readonly RequestRegisterOtpHandler _requestRegisterOtpHandler;
        private readonly VerifyRegisterOtpHandler _verifyRegisterOtpHandler;

        public AbhaRegisterController(
            RequestRegisterOtpHandler requestRegisterOtpHandler,
            VerifyRegisterOtpHandler verifyRegisterOtpHandler
        )
        {
            _requestRegisterOtpHandler = requestRegisterOtpHandler;
            _verifyRegisterOtpHandler = verifyRegisterOtpHandler;
        }

        [HttpPost("request-otp-register")]
        public async Task<ActionResult<OtpResponse>> RequestOtpRegister([FromBody] RequestRegisterOtpCommand command)
        {
            try
            {
                if (string.IsNullOrEmpty(command.AadhaarNumber))
                    return BadRequest(new { Message = "Aadhaar number is required." });

                var result = await _requestRegisterOtpHandler.Handle(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("verify-otp-register")]
        public async Task<ActionResult<VerifyRegisterOtpResponse>> VerifyOtpRegister([FromBody] VerifyRegisterOtpCommand command)
        {
            try
            {
                if (string.IsNullOrEmpty(command.TxnId) || string.IsNullOrEmpty(command.Otp) || string.IsNullOrEmpty(command.Mobile))
                    return BadRequest(new { Message = "TxnId, OTP, and Mobile are required." });

                var result = await _verifyRegisterOtpHandler.Handle(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }
    }
}
