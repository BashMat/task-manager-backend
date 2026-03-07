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
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, 
                          ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUserData()
    {
        _logger.LogTrace("Start GET /api/users/current request processing");
        
        ServiceResponse<GetUserDataResponse> response = await _userService.GetUserDataById(UserId, 
                                                                                           UserId);
        
        _logger.LogTrace("Finish GET /api/users/current request processing");
        
        return HandleServiceResponse(response);
    }
    
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserDataById([FromRoute] int userId)
    {
        _logger.LogTrace($"Start GET /api/users/{userId} request processing");
        
        ServiceResponse<GetUserDataResponse> response = await _userService.GetUserDataById(UserId, 
                                                                                           userId);
        
        _logger.LogTrace($"Finish GET /api/users/{userId} request processing");
        
        return HandleServiceResponse(response);
    }
    
    [HttpPost("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        _logger.LogTrace("Start POST /api/users/update-password request processing");
        
        ServiceResponse<bool> response = await _userService.UpdatePassword(UserId, 
                                                                           request);
        
        _logger.LogTrace("Finish POST /api/users/update-password request processing");
        
        return HandleServiceResponse(response);
    }
}