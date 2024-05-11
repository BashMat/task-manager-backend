using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.Dto.User;

namespace TaskManagerBackend.Application.Controllers
{
    [Route("api/auth")]
    [ApiController]
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

        [EnableCors("MyDefaultPolicy")]
        [HttpPost("signup")]
        public async Task<ActionResult<ServiceResponse<UserSignUpResponseDto>>> SignUp([FromBody] UserSignUpRequestDto requestData)
        {
            _logger.LogTrace("Start processing POST /api/auth/signup request");
            
            ServiceResponse<UserSignUpResponseDto> response = await _authService.SignUp(requestData);
            
            _logger.LogTrace("Finish processing POST /api/auth/signup request");
            
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
            _logger.LogTrace("Start POST /api/auth/login request processing");
            
            ServiceResponse<string> response = await _authService.LogIn(requestData);
            
            _logger.LogTrace("Finish POST /api/auth/login request processing");
            
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
