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
        ServiceResponse<TrackingLogGetResponse> response = await trackingService.CreateTrackingLog(request,
                                                                                                   UserId,
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
        return HandleServiceResponse(await trackingService.GetTrackingLogById(id,
                                                                              UserId,
                                                                              cancellationToken));
    }
    
    [HttpPost("logs/edit")]
    public async Task<IActionResult> EditTrackingLog([FromBody] TrackingLogEditRequest request,
                                                     CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.EditTrackingLog(request, 
                                                                           UserId,
                                                                           cancellationToken));
    }
    
    [HttpDelete("logs/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogById([FromRoute] int id,
                                                           CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogById(id,
                                                                                 UserId,
                                                                                 cancellationToken));
    }

    #endregion
    
    #region Tracking Log Entries
    
    [HttpPost("log-entries")]
    public async Task<IActionResult> CreateTrackingLogEntry([FromBody] TrackingLogEntryCreateRequest request,
                                                            CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogEntryGetResponse> response = 
            await trackingService.CreateTrackingLogEntry(request, UserId, cancellationToken);
            
        if (response is { Data: not null, Success: true})
        {
            return CreatedAtAction(nameof(GetTrackingLogEntryById),
                                   new { id = response.Data.Id },
                                   response);
        }
        
        return HandleServiceResponse(response);
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
        return HandleServiceResponse(await trackingService.GetTrackingLogEntryById(id,
                                                                                   UserId,
                                                                                   cancellationToken));
    }
    
    [HttpPut("log-entries/{id:int}")]
    [Obsolete("Use specialized actions instead of single update action")]
    public async Task<IActionResult> UpdateTrackingLogEntryById([FromRoute] int id,
                                                                [FromBody] 
                                                                UpdateTrackingLogEntryRequest request,
                                                                CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.UpdateTrackingLogEntryById(id, 
                                                                                      request,
                                                                                      UserId,
                                                                                      cancellationToken));
    }
    
    [HttpPost("log-entries/edit")]
    public async Task<IActionResult> EditTrackingLogEntry([FromBody] 
                                                          TrackingLogEntryEditRequest request,
                                                          CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.EditTrackingLogEntry(request,
                                                                                UserId,
                                                                                cancellationToken));
    }
    
    [HttpPost("log-entries/move")]
    public async Task<IActionResult> MoveTrackingLogEntry([FromBody] 
                                                          TrackingLogEntryMoveRequest request,
                                                          CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.MoveTrackingLogEntry(request,
                                                                                UserId,
                                                                                cancellationToken));
    }
    
    [HttpDelete("log-entries/{id:int}")]
    public async Task<IActionResult> DeleteTrackingLogEntryById([FromRoute] int id,
                                                                CancellationToken cancellationToken)
    {
        return HandleServiceResponse(await trackingService.DeleteTrackingLogEntryById(id, UserId, cancellationToken));
    }

    #endregion

    #region Statuses
    
    [HttpPost("statuses")]
    public async Task<IActionResult> CreateTrackingLogEntryStatus([FromBody] 
                                                                  TrackingLogEntryStatusCreateRequest request,
                                                                  CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogEntryStatusGetResponse> response = 
            await trackingService.CreateTrackingLogStatus(request,
                                                          UserId,
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
        return HandleServiceResponse(await trackingService.DeleteTrackingLogStatus(id, UserId, cancellationToken));
    }

    #endregion
}