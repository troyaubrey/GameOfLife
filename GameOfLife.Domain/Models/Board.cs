using GameOfLife.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GameOfLife.Domain.Models
{
  public class Board
  {
    public Guid? Id { get; set; }

    [Required]
    public List<List<bool>> BoardState { get; set; } = [];

    public BoardEntity ToBoardEntity()
    {
      var entity = new BoardEntity
      {
        Id = Id,
        BoardState = SerializeListOfLists(BoardState)
      };
      return entity;
    }

    public string SerializeListOfLists(List<List<bool>> listOfLists)
    {
      return JsonSerializer.Serialize(listOfLists);
    }
  }
}
