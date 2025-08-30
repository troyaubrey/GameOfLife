using GameOfLife.Application.Services;
using GameOfLife.Domain.Models;
using GameOfLife.Domain.Entities;
using GameOfLife.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameOfLife.Tests.ApplicationTests
{
  public class BoardServiceTests
  {
    private BoardContext CreateInMemoryContext()
    {
      var options = new DbContextOptionsBuilder<BoardContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString())
          .Options;
      return new BoardContext(options);
    }

    private List<List<bool>> GetSampleState()
    {
      return new List<List<bool>> { new List<bool> { true, false }, new List<bool> { false, true } };
    }

    [Fact]
    public async Task CreateBoard_AddsBoardAndReturnsIt()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var board = new Board { BoardState = GetSampleState() };
      var result = await service.CreateBoard(board);

      Assert.NotNull(result);
      Assert.NotNull(result.Id);
      Assert.Equal(board.BoardState, result.BoardState);
    }

    [Fact]
    public async Task GetAllBoards_ReturnsAllBoards()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var board1 = new Board { Id = Guid.NewGuid() , BoardState = GetSampleState()};
      var boardEntity1 = board1.ToBoardEntity();

      var board2 = new Board { Id = Guid.NewGuid(), BoardState = GetSampleState() };
      var boardEntity2 = board2.ToBoardEntity();


      context.Boards.Add(boardEntity1);
      context.Boards.Add(boardEntity2);
      await context.SaveChangesAsync();

      var result = await service.GetAllBoards();

      Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBoardById_ReturnsBoard_WhenFound()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var board = new Board { Id = Guid.NewGuid(), BoardState = GetSampleState() };
      var boardEntity = board.ToBoardEntity();

      context.Boards.Add(boardEntity);
      await context.SaveChangesAsync();

      var result = await service.GetBoardById(board.Id.Value);

      Assert.NotNull(result);
      Assert.Equal(board.Id, result.Id);
    }

    [Fact]
    public async Task GetBoardById_ReturnsNull_WhenNotFound()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var result = await service.GetBoardById(Guid.NewGuid());

      Assert.Null(result);
    }

    // For GetNextState, GetNStatesAhead, GetFinalState, you should use the real GameOfLifeEngine or mock its behavior if possible.
    // Here, we assume the engine works and just check the service logic.

    [Fact]
    public async Task GetNextState_ReturnsBoardWithNextState()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var board = new Board { Id = Guid.NewGuid(), BoardState = GetSampleState() };
      var boardEntity = board.ToBoardEntity();
      context.Boards.Add(boardEntity);
      await context.SaveChangesAsync();

      var result = await service.GetNextState(board.Id.Value);

      Assert.NotNull(result);
      Assert.NotNull(result.BoardState);
    }

    [Fact]
    public async Task GetNextState_ReturnsNull_WhenNotFound()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var result = await service.GetNextState(Guid.NewGuid());

      Assert.Null(result);
    }

    [Fact]
    public async Task GetNStatesAhead_ReturnsBoardWithNStates()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var board = new Board { Id = Guid.NewGuid(), BoardState = GetSampleState() };
      var boardEntity = board.ToBoardEntity();
      context.Boards.Add(boardEntity);
      await context.SaveChangesAsync();

      var result = await service.GetNStatesAhead(board.Id.Value, 2);

      Assert.NotNull(result);
      Assert.NotNull(result.BoardState);
    }

    [Fact]
    public async Task GetNStatesAhead_ReturnsNull_WhenNotFound()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var result = await service.GetNStatesAhead(Guid.NewGuid(), 2);

      Assert.Null(result);
    }

    [Fact]
    public async Task GetFinalState_ReturnsBoardAndStatus()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var board = new Board { Id = Guid.NewGuid(), BoardState = GetSampleState() };
      var boardEntity = board.ToBoardEntity();
      context.Boards.Add(boardEntity);
      await context.SaveChangesAsync();

      var (result, status) = await service.GetFinalState(board.Id.Value);

      Assert.NotNull(result);
      Assert.NotNull(result.BoardState);
      Assert.NotNull(status);
    }

    [Fact]
    public async Task GetFinalState_ReturnsNullAndEmpty_WhenNotFound()
    {
      using var context = CreateInMemoryContext();
      var logger = new Mock<ILogger<BoardService>>();
      var service = new BoardService(context, logger.Object);

      var (result, status) = await service.GetFinalState(Guid.NewGuid());

      Assert.Null(result);
      Assert.Equal(string.Empty, status);
    }
  }
}




