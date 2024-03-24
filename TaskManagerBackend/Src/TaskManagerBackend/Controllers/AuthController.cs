using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Dto.User;
using TaskManagerBackend.Services.Auth;

namespace TaskManagerBackend.Controllers
{
    [Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[EnableCors("MyDefaultPolicy")]
		[HttpPost("signup")]
		public async Task<ActionResult<ServiceResponse<UserSignUpResponseDto>>> SignUp([FromBody] UserSignUpRequestDto requestData)
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
		public async Task<ActionResult<ServiceResponse<string>>> LogIn([FromBody] UserLogInRequestDto requestData)
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
