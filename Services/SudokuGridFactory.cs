using SudokuSolver.Web.Models;

namespace SudokuSolver.Web.Services;

public class SudokuGridFactory : ISudokuGridFactory
{
    public GridContext CreateRandomGrid()
    {
        var cells = new List<int>();

        for (var i = 0; i < Settings.TotalCellsCount; i++)
        {
            cells.Add(Random.Shared.Next(Settings.MinCellValue, Settings.MaxCellValue));
        }

        return new GridContext
        {
            Cells = cells
        };
    }
}