#region Usings

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Services.Board;
using TaskManagerBackend.Dto.Board;
using TaskManagerBackend.Dto.Card;
using TaskManagerBackend.Dto.Column;

#endregion

namespace TaskManagerBackend.Application.Controllers;

[Route("api/boards")]
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
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> Create([FromBody] BoardCreateRequestDto newBoard)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ServiceResponse<BoardGetResponseDto> response = await _boardService.Create(userId, newBoard);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(Create), response);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<BoardGetResponseDto>>>> GetAll()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(await _boardService.GetAll(userId));
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> GetById([FromRoute] int id)
    {
        ServiceResponse<BoardGetResponseDto> response = await _boardService.GetById(id);
            
        if (response.Success)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse<BoardGetResponseDto>>> Update([FromRoute] int id,
        [FromBody] BoardUpdateRequestDto updatedBoard)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
        ServiceResponse<BoardGetResponseDto> response = await _boardService.Update(userId, id, updatedBoard);
            
        if (response.Success)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<List<BoardGetResponseDto>>>> Delete([FromRoute] int id)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(await _boardService.Delete(userId, id));
    }

    #endregion

    #region Column

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("columns")]
    public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> CreateColumn([FromBody] ColumnCreateRequestDto newColumn)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
        ServiceResponse<ColumnGetResponseDto> response = await _boardService.CreateColumn(userId, newColumn);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateColumn), response);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("columns")]
    public async Task<ActionResult<ServiceResponse<List<ColumnGetResponseDto>>>> GetAllColumns()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(await _boardService.GetAllColumns(userId));
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("columns/{id}")]
    public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> GetColumnById([FromRoute] int id)
    {
        ServiceResponse<ColumnGetResponseDto> response = await _boardService.GetColumnById(id);
            
        if (response.Success)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPut("columns/{id}")]
    public async Task<ActionResult<ServiceResponse<ColumnGetResponseDto>>> UpdateColumn([FromRoute] int id,
        [FromBody] ColumnUpdateRequestDto updatedColumn)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
        ServiceResponse<ColumnGetResponseDto> response = await _boardService.UpdateColumn(userId, id, updatedColumn);
            
        if (response.Success)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpDelete("columns/{id}")]
    public async Task<ActionResult<ServiceResponse<List<ColumnGetResponseDto>>>> DeleteColumn([FromRoute] int id)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(await _boardService.DeleteColumn(userId, id));
    }

    #endregion

    #region Card

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPost("cards")]
    public async Task<ActionResult<ServiceResponse<CardGetResponseDto>>> CreateCard([FromBody] CardCreateRequestDto newCard)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
        ServiceResponse<CardGetResponseDto> response = await _boardService.CreateCard(userId, newCard);
            
        if (response.Success)
        {
            return CreatedAtAction(nameof(CreateCard), response);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("cards")]
    public async Task<ActionResult<ServiceResponse<List<CardGetResponseDto>>>> GetAllCards()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(await _boardService.GetAllCards(userId));
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpGet("cards/{id}")]
    public async Task<ActionResult<ServiceResponse<CardGetResponseDto>>> GetCardById([FromRoute] int id)
    {
        ServiceResponse<CardGetResponseDto> response = await _boardService.GetCardById(id);
            
        if (response.Success)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpPut("cards/{id}")]
    public async Task<ActionResult<ServiceResponse<CardGetResponseDto>>> UpdateCard([FromRoute] int id,
        [FromBody] CardUpdateRequestDto updatedCard)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ServiceResponse<CardGetResponseDto> response = await _boardService.UpdateCard(userId, id, updatedCard);
            
        if (response.Success)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [EnableCors("MyDefaultPolicy")]
    [Authorize]
    [HttpDelete("cards/{id}")]
    public async Task<ActionResult<ServiceResponse<List<CardGetResponseDto>>>> DeleteCard([FromRoute] int id)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return Ok(await _boardService.DeleteCard(userId, id));
    }

    #endregion
}