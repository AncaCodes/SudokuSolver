namespace SudokuSolver.Web.Models;

public class LocalGrid
{
    public Cell[,] Cells { get; set; } = new Cell[3, 3];

    public Cell GetCellAt(int x, int y)
    {
        return Cells[x, y];
    }
}