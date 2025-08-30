using GameOfLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameOfLife.Infrastructure
{
  public class BoardContext(DbContextOptions<BoardContext> options)
    : DbContext(options)
  {
    public DbSet<BoardEntity> Boards => Set<BoardEntity>();

  }
}
