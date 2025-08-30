using GameOfLife.Application.Engines;

namespace GameOfLife.Tests.ApplicationTests
{
  public class GameOfLifeTests
  {
    // Helper to create a grid from a string array
    private List<List<bool>> CreateGrid(params string[] rows)
    {
      var grid = new List<List<bool>>();
      foreach (var row in rows)
      {
        var list = new List<bool>();
        foreach (var c in row)
          list.Add(c == '1');
        grid.Add(list);
      }
      return grid;
    }

    [Fact]
    public void GetNStatesAhead_AdvancesCorrectly()
    {
      // Blinker pattern (oscillator)
      var initial = CreateGrid(
          "000",
          "111",
          "000"
      );
      var expected = CreateGrid(
          "010",
          "010",
          "010"
      );
      var result = GameOfLifeEngine.GetNStatesAhead(initial, 1);
      Assert.Equal(expected, result);
    }

    [Fact]
    public void GetFinalStableState_StillLife_ReturnsStable()
    {
      // Block pattern (still life)
      var initial = CreateGrid(
          "1100",
          "1100",
          "0000",
          "0000"
      );
      var (state, status) = GameOfLifeEngine.GetFinalStableState(initial, 10);
      Assert.Equal("stable", status);
      Assert.Equal(initial, state);
    }

    [Fact]
    public void GetFinalStableState_Oscillator_ReturnsCycle()
    {
      // Blinker pattern (oscillator)
      var initial = CreateGrid(
          "000",
          "111",
          "000"
      );
      var (state, status) = GameOfLifeEngine.GetFinalStableState(initial, 10);
      Assert.Equal("cycle", status);
    }

    [Fact]
    public void NextGeneration_UpdatesGridCorrectly()
    {
      var current = CreateGrid(
          "000",
          "111",
          "000"
      );
      var next = CreateGrid(
          "000",
          "000",
          "000"
      );
      GameOfLifeEngine.NextGeneration(current, next);
      var expected = CreateGrid(
          "010",
          "010",
          "010"
      );
      Assert.Equal(expected, next);
    }

    [Theory]
    [InlineData(1, 1, true)]  // Center cell in blinker, should stay alive
    [InlineData(0, 1, true)]  // Top cell in blinker, should become alive
    [InlineData(1, 0, false)] // Left cell in blinker, should die
    public void GetNextState_ComputesCorrectState(int row, int col, bool expected)
    {
      var grid = CreateGrid(
          "000",
          "111",
          "000"
      );
      var result = GameOfLifeEngine.GetNextState(grid, row, col);
      Assert.Equal(expected, result);
    }

    [Fact]
    public void CloneGrid_CreatesDeepCopy()
    {
      var original = CreateGrid(
          "01",
          "10"
      );
      var clone = typeof(GameOfLifeEngine)
          .GetMethod("CloneGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
          .Invoke(null, new object[] { original }) as List<List<bool>>;

      Assert.Equal(original, clone);
      clone[0][0] = !clone[0][0];
      Assert.NotEqual(original, clone);
    }

    [Fact]
    public void GridSignature_ProducesConsistentHash()
    {
      var grid = CreateGrid(
          "01",
          "10"
      );
      var signature = typeof(GameOfLifeEngine)
          .GetMethod("GridSignature", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
          .Invoke(null, new object[] { grid }) as string;

      Assert.Equal("01;10", signature);
    }
  }
}
