using Asp.netWebAPP.Core.Application.M2.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netWebAPP.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BridgeController : Controller
    {
        private readonly IMediator _mediator;

        public BridgeController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost("register-service")]
        public async Task<IActionResult> RegisterService([FromBody] RegisterBridgeServiceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
