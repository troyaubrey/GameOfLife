using GameOfLife.Application.Services;
using GameOfLife.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameOfLife.ApiService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BoardsController : ControllerBase
  {
    private readonly IBoardService _boardService;

    public BoardsController(IBoardService boardService)
    {
      _boardService = boardService;
    }

    // POST: api/Boards
    [HttpPost]
    public async Task<ActionResult<Board>> PostBoard(Board board)
    {
      if (board == null)
      { 
        return BadRequest("Request body is null or invalid"); 
      }

      if (board.BoardState == null || board.BoardState.Count == 0 || board.BoardState.All(innerList => innerList.Count == 0))
      {
        return BadRequest(ModelState);
      }

      var newBoard = await _boardService.CreateBoard(board);

      return CreatedAtAction("GetBoard", new { id = newBoard.Id }, new { id = newBoard.Id });
    }

    // GET: api/Boards
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
    {
      return await _boardService.GetAllBoards();
    }

    // GET: api/Boards/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Board>> GetBoard(Guid id)
    {
      var board = await _boardService.GetBoardById(id);

      if (board == null)
      {
        return NotFound();
      }

      return board;
    }

    // GET: api/Boards/5/next
    [HttpGet("{id}/next")]
    public async Task<ActionResult<Board>> GetNextState(Guid id)
    {
      var board = await _boardService.GetNextState(id);

      if (board == null)
      {
        return NotFound();
      }

      return board;
    }

    // GET: api/Boards/5/future/20
    [HttpGet("{id}/future/{n}")]
    public async Task<ActionResult<Board>> GetNStatesAhead(Guid id, int n)
    {
      var board = await _boardService.GetNStatesAhead(id, n);

      if (board == null)
      {
        return NotFound();
      }

      return board;
    }

    //GET: api/Boards/5/final
    [HttpGet("{id}/final")]
    public async Task<ActionResult<Board>> GetFinalState(Guid id)
    {
      var (board, status) = await _boardService.GetFinalState(id);

      if (board == null)
      {
        return NotFound();
      }

      if (status == "max_iterations_reached")
      {
        return UnprocessableEntity(new ValidationProblemDetails(ModelState)
        {
          Title = "Unprocessable Entity",
          Detail = "The board could not reach a final stable state within 1000 iterations.",
          Status = (int)HttpStatusCode.UnprocessableEntity // Explicitly set status
        });
      }

      return board;
    }
  }
}
