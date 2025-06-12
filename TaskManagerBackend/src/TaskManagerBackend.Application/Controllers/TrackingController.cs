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

[ApiController]
[Route("api/tracking")]
[EnableCors]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;
    
    public TrackingController(ITrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    #region Tracking Logs
    
    [HttpPost("logs")]
    public async Task<IActionResult> CreateTrackingLog([FromBody] TrackingLogCreateRequest newLog)
    {
        ServiceResponse<TrackingLogGetResponse> response = await _trackingService.CreateTrackingLog(UserId, newLog);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpGet("logs")]
    public async Task<IActionResult> GetAllTrackingLogs()
    {
        return Ok(await _trackingService.GetAllTrackingLogsByUserId(UserId));
    }
    
    [HttpGet("logs/{id:int}")]
    public async Task<IActionResult> GetTrackingLogById([FromRoute] int id)
    {
        ServiceResponse<TrackingLogGetResponse> response = await _trackingService.GetTrackingLogById(id);

        if (response.Success)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpDelete("logs/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogById([FromRoute] int id)
    {
        return Ok(await _trackingService.DeleteTrackingLogById(UserId, id));
    }

    #endregion
    
    #region Tracking Log Entries
    
    [HttpPost("log-entries")]
    public async Task<IActionResult> CreateTrackingLogEntry([FromBody] TrackingLogEntryCreateRequest newLogEntry)
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
    
    [HttpGet("log-entries")]
    public async Task<IActionResult> GetAllTrackingLogEntries()
    {
        return Ok(await _trackingService.GetAllTrackingLogEntriesByUserId(UserId));
    }
    
    [HttpGet("log-entries/{id:int}")]
    public async Task<IActionResult> GetTrackingLogEntryById([FromRoute] int id)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = 
            await _trackingService.GetTrackingLogEntryById(id);

        if (response.Success)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpPut("log-entries/{id:int}")]
    public async Task<IActionResult> UpdateTrackingLogEntryById([FromRoute] int id,
                                                                [FromBody] UpdateTrackingLogEntryRequest updatedTrackingLogEntry)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = 
            await _trackingService.UpdateTrackingLogEntry(UserId,
                                                          id,
                                                          updatedTrackingLogEntry);

        if (response.Success)
        {
            return Ok(response);
        }

        return NotFound(response);
    }
    
    [HttpDelete("log-entries/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogEntryById([FromRoute] int id)
    {
        return Ok(await _trackingService.DeleteTrackingLogEntryById(UserId, id));
    }

    #endregion

    #region Statuses
    
    [HttpPost("statuses")]
    public async Task<IActionResult> CreateTrackingLogEntryStatus([FromBody] TrackingLogEntryStatusCreateRequest newStatus)
    {
        ServiceResponse<TrackingLogEntryStatus> response = await _trackingService.CreateTrackingLogStatus(UserId, 
                                                                                                          newStatus);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpDelete("statuses/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogEntryStatus([FromRoute] int id)
    {
        return Ok(await _trackingService.DeleteTrackingLogStatus(UserId, id));
    }

    #endregion
}