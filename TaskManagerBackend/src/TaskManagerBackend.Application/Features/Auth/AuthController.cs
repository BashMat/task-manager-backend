﻿#region Usings

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
    
    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] UserLogInRequest requestData)
    {
        _logger.LogTrace("Start POST /api/auth/login request processing");
            
        ServiceResponse<string> response = await _authService.LogIn(requestData);
            
        _logger.LogTrace("Finish POST /api/auth/login request processing");
            
        return ConvertServiceResponse(response);
    }
}