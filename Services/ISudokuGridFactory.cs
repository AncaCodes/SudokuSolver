using SudokuSolver.Web.Models;

namespace SudokuSolver.Web.Services;

public interface ISudokuGridFactory
{

    GridContext CreateRandomGrid();

    GridContext CreateEasyGrid();

    GridContext CreateMediumGrid();

    GridContext CreateHardGrid();

}