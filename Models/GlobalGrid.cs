namespace SudokuSolver.Web.Models;

public class GlobalGrid
{
    public LocalGrid[,] LocalGrids { get; set; } = new LocalGrid[3, 3];

    public Cell GetCellByGlobalIndex(int globalX, int globalY)
    {
        var gridX = globalX / 3;
        var gridY = globalY / 3;

        var grid = LocalGrids[gridX, gridY];
        return grid.GetCellAt(globalX % 3, globalY % 3);
    }

    public bool HasValueOnRow(int rowIndex, int value)
    {
        for (var i = 0; i < 9; i++)
        {
            var cell = GetCellByGlobalIndex(rowIndex, i);
            if (cell.Value == value)
            {
                return true;
            }
        }

        return false;
    }
}