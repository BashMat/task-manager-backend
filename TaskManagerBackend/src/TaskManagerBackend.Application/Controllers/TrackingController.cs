#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Dto.Tracking.TrackingLog;

#endregion

namespace TaskManagerBackend.Application.Controllers;

[Route("api/tracking")]
[ApiController]
public class TrackingController : ControllerBase
{
    public TrackingController() { }

    #region Tracking Logs

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("logs")]
    public async Task<ActionResult<ServiceResponse<TrackingLogGetResponse>>> Create([FromBody] TrackingLogCreateRequest newLog)
    {
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("logs")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogGetResponse>>>> GetAll()
    {
        return Ok();
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("logs/{id}")]
    public async Task<ActionResult<ServiceResponse<TrackingLogGetResponse>>> GetById([FromRoute] int id)
    {
        return NotFound();
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpDelete("logs/{id}")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogGetResponse>>>> Delete([FromRoute] int id)
    {
        return Ok();
    }

    #endregion
}