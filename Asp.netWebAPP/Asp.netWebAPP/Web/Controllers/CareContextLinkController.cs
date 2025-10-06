using Asp.netWebAPP.Core.Application.DTO_s;
using Asp.netWebAPP.Core.Application.M2.Commands;
using Asp.netWebAPP.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netWebAPP.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CareContextLinkController : Controller
    {
        private readonly IMediator _mediator;

        private readonly DanpheDbContext _DanpheDbContext;

        public CareContextLinkController(IMediator mediator,DanpheDbContext danpheDbContext)
        {
            _mediator = mediator;
            _DanpheDbContext = danpheDbContext;
        }

        [HttpPost("linkcarecontext")]
        public async Task<IActionResult> LinkCareContext([FromBody] CareContextLinkRequestDTO request)
        {
            var result = await _mediator.Send(new LinkCareContextCommand  { Request = request });
            return Ok(result);
        }
        [HttpGet("check-db")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                bool canConnect = await _DanpheDbContext.Database.CanConnectAsync();
                int patientCount = await _DanpheDbContext.Patient.CountAsync();
                return Ok(new
                {
                    CanConnect = canConnect,
                    TotalPatients = patientCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        //[HttpPost("generate-link-token")]
        //public async Task<IActionResult> GenerateLinkToken([FromBody] GenerateLinkTokenRequestDTO request)
        //{
        //    var result = await _mediator.Send(new GenerateLinkTokenCommand { Request = request });
        //    return Ok(result);
        //}
    }
}
