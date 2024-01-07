using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;
using Microsoft.AspNetCore.Cors;
using TaskManagerApi.Services.Auth;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[EnableCors("MyDefaultPolicy")]
		[HttpPost("register")]
		public async Task<ActionResult<ServiceResponse<User>>> SignUp(UserSignUpRequestDto requestData)
		{
			ServiceResponse<UserSignUpResponseDto> response = await _authService.SignUp(requestData);
			if (response.Success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}

		[EnableCors("MyDefaultPolicy")]
		[HttpPost("login")]
		public async Task<ActionResult<ServiceResponse<string>>> LogIn(UserLogInRequestDto requestData)
		{
			ServiceResponse<string> response = await _authService.LogIn(requestData);
			if (response.Success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
	}
}
