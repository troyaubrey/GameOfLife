using GameOfLife.Domain.Models;
using System.Text.Json;

namespace GameOfLife.Domain.Entities
{
  public class BoardEntity
  {
    public Guid? Id { get; set; }

    public string BoardState { get; set; } = string.Empty;

    public Board ToBoard()
    {
      var newBoard = new Board
      {
        Id = Id,
        BoardState = DeserializeListOfLists(BoardState)
      };

      return newBoard;
    }

    public List<List<bool>> DeserializeListOfLists(string boardState)
    {
      return JsonSerializer.Deserialize<List<List<bool>>>(boardState);
    }
  }
}
