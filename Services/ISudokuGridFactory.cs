using SudokuSolver.Web.Models;

namespace SudokuSolver.Web.Services;

public interface ISudokuGridFactory
{
    GridContext CreateRandomGrid();
}