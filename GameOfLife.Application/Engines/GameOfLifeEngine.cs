namespace GameOfLife.Application.Engines
{
  public class GameOfLifeEngine
  {
    /// <summary>
    /// Advance n steps using double buffering
    /// </summary>
    /// <param name="initial"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static List<List<bool>> GetNStatesAhead(List<List<bool>> initial, int n = 1)
    {
      int rows = initial.Count;
      int cols = initial[0].Count;

      // Two buffers to flip between
      var current = CloneGrid(initial);
      var next = CloneGrid(initial);

      for (int i = 0; i < n; i++)
      {
        NextGeneration(current, next);

        // Swap references (no copy needed)
        var temp = current;
        current = next;
        next = temp;
      }

      return current;
    }

    /// <summary>
    /// Runs until the board reaches a still life or cycles.
    /// Stops immediately at the first cycle.
    /// </summary>
    /// <param name="initial"></param>
    /// <param name="maxIterations"></param>
    /// <returns></returns>
    public static (List<List<bool>> state, string status)
        GetFinalStableState(List<List<bool>> initial, int maxIterations = 1000)
    {
      var current = CloneGrid(initial);
      var next = CloneGrid(initial);

      var seen = new Dictionary<string, int>();
      string signature = GridSignature(current);
      seen[signature] = 0;

      for (int step = 1; step <= maxIterations; step++)
      {
        NextGeneration(current, next);

        // Swap references
        var temp = current;
        current = next;
        next = temp;

        signature = GridSignature(current);

        if (seen.TryGetValue(signature, out int value))
        {
          int cycleLength = step - value;
          string status = cycleLength == 1 ? "stable" : "cycle";
          return (current, status);
        }

        seen[signature] = step;
      }

      return (current, "max_iterations_reached");
    }

    /// <summary>
    /// Compute next generation into a preallocated buffer
    /// </summary>
    /// <param name="current"></param>
    /// <param name="next"></param>
    public static void NextGeneration(List<List<bool>> current, List<List<bool>> next)
    {
      int rows = current.Count;
      int cols = current[0].Count;

      for (int row = 0; row < rows; row++)
      {
        for (int col = 0; col < cols; col++)
        {
          next[row][col] = GetNextState(current, row, col);
        }
      }
    }

    /// <summary>
    /// Compute one cell’s next state
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static bool GetNextState(List<List<bool>> grid, int row, int col)
    {
      int rows = grid.Count; 
      int cols = grid[0].Count;
      int liveNeighbors = 0;

      for (int dr = -1; dr <= 1; dr++)
      {
        for (int dc = -1; dc <= 1; dc++)
        {
          if (dr == 0 && dc == 0)
            continue;
          int nr = row + dr; 
          int nc = col + dc;
          if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && grid[nr][nc])
            liveNeighbors++;
        }
      }

      bool isAlive = grid[row][col];
      if (isAlive)
        return liveNeighbors == 2 || liveNeighbors == 3;
      else
        return liveNeighbors == 3;
    }

    /// <summary>
    /// Clones a grid to another 
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private static List<List<bool>> CloneGrid(List<List<bool>> grid)
    {
      return grid.Select(row => row.ToList()).ToList();
    }

    /// <summary>
    /// Hashable string representation of a grid (for cycle detection)
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private static string GridSignature(List<List<bool>> grid)
    {
      return string.Join(";", grid.Select(row => string.Concat(row.Select(cell => cell ? "1" : "0"))));
    }
  }
}
