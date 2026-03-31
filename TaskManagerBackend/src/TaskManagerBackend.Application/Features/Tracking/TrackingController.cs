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
    public async Task<IActionResult> CreateTrackingLog([FromBody] TrackingLogCreateRequest request,
                                                       CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogGetResponse> response = await trackingService.CreateTrackingLog(UserId,
                                                                                                   request,
                                                                                                   cancellationToken);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return HandleServiceResponse(response);
    }
    
    [HttpGet("logs")]
    public async Task<IActionResult> GetAllTrackingLogs(CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.GetAllTrackingLogsByUserId(UserId, cancellationToken));
    }
    
    [HttpGet("logs/{id:int}")]
    public async Task<IActionResult> GetTrackingLogById([FromRoute] int id,
                                                        CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.GetTrackingLogById(id, cancellationToken));
    }
    
    [HttpPost("logs/edit")]
    public async Task<IActionResult> EditTrackingLog([FromBody] TrackingLogEditRequest request,
                                                     CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.EditTrackingLog(UserId, 
                                                                           request, 
                                                                           cancellationToken));
    }
    
    [HttpDelete("logs/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogById([FromRoute] int id,
                                                           CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogById(UserId, id, cancellationToken));
    }

    #endregion
    
    #region Tracking Log Entries
    
    [HttpPost("log-entries")]
    public async Task<IActionResult> CreateTrackingLogEntry([FromBody] TrackingLogEntryCreateRequest request,
                                                            CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = 
            await trackingService.CreateTrackingLogEntry(UserId, request, cancellationToken);
            
        if (response.Data is not null && response.Success)
        {
            return CreatedAtAction(nameof(GetTrackingLogEntryById),
                                   new { id = response.Data.Id },
                                   response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpGet("log-entries")]
    public async Task<IActionResult> GetAllTrackingLogEntries(CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.GetAllTrackingLogEntriesByUserId(UserId, cancellationToken));
    }
    
    [HttpGet("log-entries/{id:int}")]
    public async Task<IActionResult> GetTrackingLogEntryById([FromRoute] int id,
                                                             CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.GetTrackingLogEntryById(id, cancellationToken));
    }
    
    [HttpPut("log-entries/{id:int}")]
    public async Task<IActionResult> UpdateTrackingLogEntryById([FromRoute] int id,
                                                                [FromBody] 
                                                                UpdateTrackingLogEntryRequest request,
                                                                CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.UpdateTrackingLogEntry(UserId,
                                                                                  id,
                                                                                  request,
                                                                                  cancellationToken));
    }
    
    [HttpDelete("log-entries/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogEntryById([FromRoute] int id,
                                                                CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogEntryById(UserId, id, cancellationToken));
    }

    #endregion

    #region Statuses
    
    [HttpPost("statuses")]
    public async Task<IActionResult> CreateTrackingLogEntryStatus([FromBody] 
                                                                  TrackingLogEntryStatusCreateRequest request,
                                                                  CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogEntryStatusGetResponse> response = 
            await trackingService.CreateTrackingLogStatus(UserId,
                                                          request,
                                                          cancellationToken);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateTrackingLog), response);
        }
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpDelete("statuses/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogEntryStatus([FromRoute] int id,
                                                                  CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogStatus(UserId, id, cancellationToken));
    }

    #endregion
}