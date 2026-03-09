#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.User;

[ApiController]
[Route("api/users")]
[EnableCors]
[Authorize]
public class UserController(IUserService userService,
                            ILogger<UserController> logger) : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUserData()
    {
        logger.LogTrace("Start GET /api/users/current request processing");
        
        ServiceResponse<GetUserDataResponse> response = await userService.GetUserDataById(UserId, 
                                                                                           UserId);
        
        logger.LogTrace("Finish GET /api/users/current request processing");
        
        return HandleServiceResponse(response);
    }
    
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserDataById([FromRoute] int userId)
    {
        logger.LogTrace($"Start GET /api/users/{userId} request processing");
        
        ServiceResponse<GetUserDataResponse> response = await userService.GetUserDataById(UserId, 
                                                                                           userId);
        
        logger.LogTrace($"Finish GET /api/users/{userId} request processing");
        
        return HandleServiceResponse(response);
    }
    
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        logger.LogTrace("Start POST /api/users/update-password request processing");
        
        ServiceResponse<bool> response = await userService.UpdatePassword(UserId, 
                                                                           request);
        
        logger.LogTrace("Finish POST /api/users/update-password request processing");
        
        return HandleServiceResponse(response);
    }
}