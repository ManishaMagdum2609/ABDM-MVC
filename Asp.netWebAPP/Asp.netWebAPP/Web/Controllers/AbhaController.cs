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
    public class AbhaController : ControllerBase
    {
        private readonly SearchAbhaHandler _searchHandler;
        private readonly RequestOtpLoginHandler _otpHandler;
        private readonly VerifyOtpHandler _verifyHandler;
        private readonly SearchPatientByMobileHandler _searchPatientHandler;

        public AbhaController(
            SearchAbhaHandler searchHandler,
            RequestOtpLoginHandler otpHandler,
            VerifyOtpHandler verifyHandler,
            SearchPatientByMobileHandler patientHandler)
        {
            _searchHandler = searchHandler;
            _otpHandler = otpHandler;
            _verifyHandler = verifyHandler;
            _searchPatientHandler = patientHandler;
        }

        [HttpPost("search-abha")]
        public async Task<ActionResult<List<AbhaAccount>>> SearchAbha([FromBody] SearchAbhaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Mobile))
                    return BadRequest(new { Message = "Mobile number is required." });

                var result = await _searchHandler.Handle(new SearchAbhaQuery(request.Mobile));

                if (result == null || !result.Any())
                    return NotFound(new { Message = "No ABHA account found for the given mobile number." });

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

        [HttpPost("request-otp-login")]
        public async Task<ActionResult<OtpResponse>> RequestOtpLogin([FromBody] RequestOtpLoginCommand command)
        {
            try
            {
                var result = await _otpHandler.Handle(command);
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

        [HttpPost("verify-abha-login")]
        public async Task<ActionResult<VerifyOtpResponse>> VerifyAbhaLogin([FromBody] VerifyOtpCommand command)
        {
            try
            {
                var result = await _verifyHandler.Handle(command);
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

        [HttpPost("search-patient")]
        public async Task<ActionResult<List<PatientSerachDTO>>> SearchPatient([FromBody] SearchAbhaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Mobile))
                    return BadRequest(new { Message = "Mobile number is required." });

                var result = await _searchPatientHandler.Handle(new SearchPatientByMobileQuery(request.Mobile));

                if (result == null)
                    return NotFound(new { Message = "No patient found for the given mobile number." });

                return Ok(new[] { result });
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
