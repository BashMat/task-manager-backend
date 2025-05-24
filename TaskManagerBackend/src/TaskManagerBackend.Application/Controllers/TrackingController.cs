#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Services.Tracking;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
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
    public async Task<ActionResult<ServiceResponse<TrackingLogGetResponse>>> CreateTrackingLog(
        [FromBody] TrackingLogCreateRequest newLog)
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
        return Ok(await _trackingService.GetAllTrackingLogsByUserId(UserId));
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("logs/{id:int}")]
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
    [HttpDelete("logs/{id:int}")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogGetResponse>>>> DeleteTrackingLogById(
        [FromRoute] int id)
    {
        return Ok(await _trackingService.DeleteTrackingLogById(UserId, id));
    }

    #endregion
    
    #region Tracking Log Entries

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("log-entries")]
    public async Task<ActionResult<ServiceResponse<TrackingLogGetResponse>>> CreateTrackingLogEntry(
        [FromBody] TrackingLogEntryCreateRequest newLogEntry)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = 
            await _trackingService.CreateTrackingLogEntry(UserId, newLogEntry);
            
        if (response.Data is not null && response.Success)
        {
            return CreatedAtAction(nameof(GetTrackingLogEntryById),
                                   new { id = response.Data.Id },
                                   response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("log-entries")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogGetResponse>>>> GetAllTrackingLogEntries()
    {
        return Ok(await _trackingService.GetAllTrackingLogEntriesByUserId(UserId));
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("log-entries/{id:int}")]
    public async Task<ActionResult<ServiceResponse<TrackingLogEntryGetResponse>>> GetTrackingLogEntryById(
        [FromRoute] int id)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = await _trackingService.GetTrackingLogEntryById(id);

        if (response.Success)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpDelete("log-entries/{id:int}")]
    public async Task<ActionResult<ServiceResponse<List<TrackingLogEntryGetResponse>>>> DeleteTrackingLogEntryById(
        [FromRoute] int id)
    {
        return Ok(await _trackingService.DeleteTrackingLogEntryById(UserId, id));
    }

    #endregion

    #region Statuses

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("statuses")]
    public async Task<ActionResult<ServiceResponse<TrackingLogEntryStatus>>> CreateTrackingLogEntryStatus(
        [FromBody] TrackingLogEntryStatusCreateRequest newStatus)
    {
        ServiceResponse<TrackingLogEntryStatus> response = await _trackingService.CreateTrackingLogStatus(UserId, 
                                                                                                          newStatus);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    #endregion
}