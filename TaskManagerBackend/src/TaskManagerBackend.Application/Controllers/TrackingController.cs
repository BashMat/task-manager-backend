#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Services.Tracking;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Controllers;

[Route("api/tracking")]
[ApiController]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;
    
    public TrackingController(ITrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    #region Tracking Logs

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("logs")]
    public async Task<ActionResult<ServiceResponse<TrackingLogGetResponse>>> CreateTrackingLog([FromBody] TrackingLogCreateRequest newLog)
    {
        ServiceResponse<TrackingLogGetResponse> response = await _trackingService.CreateTrackingLog(UserId, newLog);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("logs")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogGetResponse>>>> GetAllTrackingLogs()
    {
        return Ok(await _trackingService.GetAllTrackingLogs(UserId));
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("logs/{id}")]
    public async Task<ActionResult<ServiceResponse<TrackingLogGetResponse>>> GetTrackingLogById([FromRoute] int id)
    {
        ServiceResponse<TrackingLogGetResponse> response = await _trackingService.GetTrackingLogById(id);

        if (response.Success)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpDelete("logs/{id}")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogGetResponse>>>> DeleteTrackingLogById([FromRoute] int id)
    {
        return Ok(await _trackingService.DeleteTrackingLogById(UserId, id));
    }

    #endregion

    #region Statuses

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("statuses")]
    public async Task<ActionResult<ServiceResponse<TrackingLogEntryStatus>>> CreateTrackingLogEntryStatus([FromBody] TrackingLogEntryStatusCreateRequest newStatus)
    {
        ServiceResponse<TrackingLogEntryStatus> response = await _trackingService.CreateTrackingLogStatus(UserId, newStatus);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    #endregion
}