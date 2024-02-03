using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Domain.Dtos.Board;
using TaskManagerApi.Domain;
using System.Security.Claims;
using TaskManagerApi.Services.Board;
using Microsoft.AspNetCore.Cors;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class BoardController : ControllerBase
	{
		private readonly IBoardService _boardService;

		public BoardController(IBoardService boardService)
		{
			_boardService = boardService;
		}

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpGet]
		public async Task<ActionResult<ServiceResponse<List<BoardGetResponseDto>>>> GetAll()
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.GetAll(userId));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> GetById([FromRoute] int id)
        {
            return Ok(await _boardService.GetById(id));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpPost]
		public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> Create([FromBody] BoardCreateRequestDto newBoard)
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.Create(userId, newBoard));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> Update([FromRoute] int id,
                                                                    [FromBody] BoardUpdateRequestDto updatedBoard)
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.Update(userId, id, updatedBoard));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpDelete("{id}")]
		public async Task<ActionResult<ServiceResponse<List<BoardGetResponseDto>>>> Delete([FromRoute] int id)
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.Delete(userId, id));
        }
	}
}
