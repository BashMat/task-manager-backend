using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Domain;
using System.Security.Claims;
using TaskManagerApi.Services.Board;
using Microsoft.AspNetCore.Cors;
using TaskManagerApi.Dto.Board;

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

        #region Board

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpPost("boards")]
        public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> Create([FromBody] BoardCreateRequestDto newBoard)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.Create(userId, newBoard));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpGet("boards")]
		public async Task<ActionResult<ServiceResponse<List<BoardGetResponseDto>>>> GetAll()
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.GetAll(userId));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpGet("boards/{id}")]
        public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> GetById([FromRoute] int id)
        {
            return Ok(await _boardService.GetById(id));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpPut("boards/{id}")]
        public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> Update([FromRoute] int id,
                                                                    [FromBody] BoardUpdateRequestDto updatedBoard)
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.Update(userId, id, updatedBoard));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
		[HttpDelete("boards/{id}")]
		public async Task<ActionResult<ServiceResponse<List<BoardGetResponseDto>>>> Delete([FromRoute] int id)
		{
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.Delete(userId, id));
        }

        #endregion

        #region Column

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpPost("columns")]
        public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> CreateColumn([FromBody] ColumnCreateRequestDto newColumn)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.CreateColumn(userId, newColumn));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpGet("columns")]
        public async Task<ActionResult<ServiceResponse<List<ColumnGetResponseDto>>>> GetAllColumns()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.GetAllColumns(userId));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpGet("columns/{id}")]
        public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> GetColumnById([FromRoute] int id)
        {
            return Ok(await _boardService.GetColumnById(id));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpPut("columns/{id}")]
        public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> UpdateColumn([FromRoute] int id,
                                                            [FromBody] ColumnUpdateRequestDto updatedColumn)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.UpdateColumn(userId, id, updatedColumn));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpDelete("columns/{id}")]
        public async Task<ActionResult<ServiceResponse<List<ColumnGetResponseDto>>>> DeleteColumn([FromRoute] int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.DeleteColumn(userId, id));
        }

        #endregion

        #region Card

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpPost("cards")]
        public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> CreateCard([FromBody] CardCreateRequestDto newCard)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.CreateCard(userId, newCard));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpGet("cards")]
        public async Task<ActionResult<ServiceResponse<List<CardGetResponseDto>>>> GetAllCards()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.GetAllCards(userId));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpGet("cards/{id}")]
        public async Task<ActionResult<ServiceResponse<CardGetResponseDto>>> GetCardById([FromRoute] int id)
        {
            return Ok(await _boardService.GetCardById(id));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpPut("cards/{id}")]
        public async Task<ActionResult<ServiceResponse<CardGetResponseDto>>> UpdateCard([FromRoute] int id,
                                                    [FromBody] CardUpdateRequestDto updatedCard)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.UpdateCard(userId, id, updatedCard));
        }

        [EnableCors("MyDefaultPolicy")]
        [Authorize]
        [HttpDelete("cards/{id}")]
        public async Task<ActionResult<ServiceResponse<List<CardGetResponseDto>>>> DeleteCard([FromRoute] int id)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(await _boardService.DeleteCard(userId, id));
        }

        #endregion
    }
}
