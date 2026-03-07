#region Usings

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Auth;

[ApiController]
[Route("api/auth")]
[EnableCors]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, 
                          ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] UserSignUpRequest requestData)
    {
        _logger.LogTrace("Start processing POST /api/auth/signup request");
            
        ServiceResponse<UserSignUpResponse> response = await _authService.SignUp(requestData);
            
        _logger.LogTrace("Finish processing POST /api/auth/signup request");

        return ConvertServiceResponse(response);
    }
    
    [HttpPost("token")]
    public async Task<IActionResult> IssueToken([FromBody] IssueTokenRequest requestData)
    {
        _logger.LogTrace($"Start POST /api/auth/token request processing for grant_type {requestData.GrantType}");
            
        ServiceResponse<IssueTokenResponse> response = await _authService.IssueToken(requestData);
            
        _logger.LogTrace($"Finish POST /api/auth/token request processing for grant_type {requestData.GrantType}");
            
        return ConvertServiceResponse(response);
    }
    
    // TODO: Add functionality to revoke token for selected device
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeTokens()
    {
        _logger.LogTrace("Start POST /api/auth/revoke request processing");
            
        await _authService.RevokeTokens(UserId);
            
        _logger.LogTrace("Finish POST /api/auth/revoke request processing");
            
        return Ok();
    }
}