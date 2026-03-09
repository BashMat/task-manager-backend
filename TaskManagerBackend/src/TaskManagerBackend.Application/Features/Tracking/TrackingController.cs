#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking;

[ApiController]
[Route("api/tracking")]
[EnableCors]
[Authorize]
public class TrackingController(ITrackingService trackingService) : ControllerBase
{
    #region Tracking Logs
    
    [HttpPost("logs")]
    public async Task<IActionResult> CreateTrackingLog([FromBody] TrackingLogCreateRequest newLog)
    {
        ServiceResponse<TrackingLogGetResponse> response = await trackingService.CreateTrackingLog(UserId, newLog);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return HandleServiceResponse(response);
    }
    
    [HttpGet("logs")]
    public async Task<IActionResult> GetAllTrackingLogs()
    {
        return HandleServiceResponse(await trackingService.GetAllTrackingLogsByUserId(UserId));
    }
    
    [HttpGet("logs/{id:int}")]
    public async Task<IActionResult> GetTrackingLogById([FromRoute] int id)
    {
        return HandleServiceResponse(await trackingService.GetTrackingLogById(id));
    }
    
    [HttpDelete("logs/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogById([FromRoute] int id)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogById(UserId, id));
    }

    #endregion
    
    #region Tracking Log Entries
    
    [HttpPost("log-entries")]
    public async Task<IActionResult> CreateTrackingLogEntry([FromBody] TrackingLogEntryCreateRequest newLogEntry)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = 
            await trackingService.CreateTrackingLogEntry(UserId, newLogEntry);
            
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
        return HandleServiceResponse(await trackingService.GetAllTrackingLogEntriesByUserId(UserId));
    }
    
    [HttpGet("log-entries/{id:int}")]
    public async Task<IActionResult> GetTrackingLogEntryById([FromRoute] int id)
    {
        return HandleServiceResponse(await trackingService.GetTrackingLogEntryById(id));
    }
    
    [HttpPut("log-entries/{id:int}")]
    public async Task<IActionResult> UpdateTrackingLogEntryById([FromRoute] int id,
                                                                [FromBody] 
                                                                UpdateTrackingLogEntryRequest updatedTrackingLogEntry)
    {
        return HandleServiceResponse(await trackingService.UpdateTrackingLogEntry(UserId,
                                                                                    id,
                                                                                    updatedTrackingLogEntry));
    }
    
    [HttpDelete("log-entries/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogEntryById([FromRoute] int id)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogEntryById(UserId, id));
    }

    #endregion

    #region Statuses
    
    [HttpPost("statuses")]
    public async Task<IActionResult> CreateTrackingLogEntryStatus([FromBody] 
                                                                  TrackingLogEntryStatusCreateRequest newStatus)
    {
        ServiceResponse<TrackingLogEntryStatusGetResponse> response = 
            await trackingService.CreateTrackingLogStatus(UserId, 
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
        return HandleServiceResponse(await trackingService.DeleteTrackingLogStatus(UserId, id));
    }

    #endregion
}