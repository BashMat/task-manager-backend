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
public class AuthController(IAuthService authService, 
                            ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] UserSignUpRequest requestData,
                                            CancellationToken cancellationToken)
    {
        logger.LogTrace("Start processing POST /api/auth/signup request");
            
        ServiceResponse<UserSignUpResponse> response = await authService.SignUp(requestData, cancellationToken);
            
        logger.LogTrace("Finish processing POST /api/auth/signup request");

        return HandleServiceResponse(response);
    }
    
    [HttpPost("token")]
    public async Task<IActionResult> IssueToken([FromBody] IssueTokenRequest requestData,
                                                CancellationToken cancellationToken)
    {
        logger.LogTrace($"Start POST /api/auth/token request processing for grant_type {requestData.GrantType}");
            
        ServiceResponse<IssueTokenResponse> response = await authService.IssueToken(requestData, cancellationToken);
            
        logger.LogTrace($"Finish POST /api/auth/token request processing for grant_type {requestData.GrantType}");
            
        return HandleServiceResponse(response);
    }
    
    // TODO: Add functionality to revoke token for selected device
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeTokens(CancellationToken cancellationToken)
    {
        logger.LogTrace("Start POST /api/auth/revoke request processing");
            
        await authService.RevokeTokens(UserId, cancellationToken);
            
        logger.LogTrace("Finish POST /api/auth/revoke request processing");
            
        return Ok();
    }
}