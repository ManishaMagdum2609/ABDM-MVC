using Asp.netWebAPP.Core.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netWebAPP.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbhaAuthController : ControllerBase
    {
        private readonly IAbhaAuthService _authService;

        public AbhaAuthController(IAbhaAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("token")]
        public async Task<ActionResult<string>> GetAccessToken()
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                return Ok(token);
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

        [HttpGet("public-key")]
        public async Task<ActionResult<string>> GetPublicKey()
        {
            try
            {
                var key = await _authService.GetPublicKeyAsync();
                return Ok(key);
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
