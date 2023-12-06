using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;
using TaskManagerApi.Services;

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

		[Authorize]
		[HttpGet("test")]
		public ActionResult Get()
		{
			return Ok();
		}

		[HttpPost("register")]
		public async Task<ActionResult<ServiceResponse<User>>> SignUp(UserDto user)
		{
			ServiceResponse<User> response = await _authService.SignUp(user);
			if (response.Success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}

		[HttpPost("login")]
		public async Task<ActionResult<ServiceResponse<string>>> Login(UserDto user)
		{
			ServiceResponse<string> response = await _authService.Login(user);
			if (response.Success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
	}
}
