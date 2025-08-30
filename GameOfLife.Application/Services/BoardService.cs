using GameOfLife.Application.Engines;
using GameOfLife.Domain.Models;
using GameOfLife.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameOfLife.Application.Services
{
  public interface IBoardService
  {
    Task<Board> CreateBoard(Board board);
    Task<List<Board>> GetAllBoards();
    Task<Board?> GetBoardById(Guid id);
    Task<Board?> GetNextState(Guid id);
    Task<Board?> GetNStatesAhead(Guid id, int n);
    Task<(Board?, string)> GetFinalState(Guid id);
  }

  public class BoardService : IBoardService
  {
    private readonly BoardContext _context;
    private readonly ILogger<BoardService> _logger;

    public BoardService(BoardContext context, ILogger<BoardService> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task<Board> CreateBoard(Board board)
    {
      var newBoard = new Board();
      try
      {
        var boardEntity = board.ToBoardEntity();
        _context.Boards.Add(boardEntity);
        await _context.SaveChangesAsync();
        newBoard = boardEntity.ToBoard();
        _logger.LogInformation("Board {BoardId} created successfully.", newBoard.Id);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating board {Board}", board);
        throw;
      } 
      return newBoard;
    }

    public async Task<List<Board>> GetAllBoards()
    {
      var boardEntities = await _context.Boards.ToListAsync();
      var boards = boardEntities.Select(boardEntity => boardEntity.ToBoard()).ToList();
      return boards;
    }

    public async Task<Board?> GetBoardById(Guid id)
    {
      var boardEntity = await _context.Boards.FindAsync(id);
      if (boardEntity == null)
      {
        return null;
      }
      return boardEntity.ToBoard();
    }

    public async Task<Board?> GetNextState(Guid id)
    {
      var boardEntity = await _context.Boards.FindAsync(id);
      if (boardEntity == null)
      {
        return null;
      }

      try
      {
        var board = boardEntity.ToBoard();
        //send to game
        board.BoardState = GameOfLifeEngine.GetNStatesAhead(board.BoardState);
        _logger.LogInformation("Board {BoardId} calculated next state successfully.", id);
        return board;
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Error getting next state for board {BoardId}", id);
        throw;
      }
    }

    public async Task<Board?> GetNStatesAhead(Guid id, int n)
    {
      var boardEntity = await _context.Boards.FindAsync(id);
      if (boardEntity == null)
      {
        return null;
      }

      try
      {
        var board = boardEntity.ToBoard();
        //run the game
        board.BoardState = GameOfLifeEngine.GetNStatesAhead(board.BoardState, n);
        _logger.LogInformation("Board {BoardId} calculated N states ahead successfully.", id);
        return board;
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Error getting N states ahead for board {BoardId}", id);
        throw;
      }
    }

    public async Task<(Board?, string)> GetFinalState(Guid id)
    {
      var boardEntity = await _context.Boards.FindAsync(id);
      if (boardEntity == null)
      {
        return (null, string.Empty);
      }

      try
      {
        var board = boardEntity.ToBoard();
        //run the game
        var (state, status) = GameOfLifeEngine.GetFinalStableState(board.BoardState);
        board.BoardState = state;
        _logger.LogInformation("Board {BoardId} calculated final state successfully.", id);
        return (board, status);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting final stable state for board {BoardId}", id);
        throw;
      }
    }
  }
}
