using Microsoft.VisualBasic;
using SudokuSolver.Web.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;

namespace SudokuSolver.Web.Services;


public class SudokuGridFactory : ISudokuGridFactory
{
    int[,] cells = new int[Settings.Dimension, Settings.Dimension];

    public GridContext CreateRandomGrid()
    {

        CheckFunctions.FillDiagonal(cells);

        CheckFunctions.FillRemaining(0, Settings.SRootDimension, cells);
        return new GridContext
        {
            Cells = cells
        };

    }
    public GridContext CreateEasyGrid()
    {
        CreateRandomGrid();
        CheckFunctions.RemoveKDigits(Random.Shared.Next(43, 49), cells);

        return new GridContext
        {
            Cells = cells
        };

    }
    public GridContext CreateMediumGrid()
    {
        CreateRandomGrid();

        CheckFunctions.RemoveKDigits(Random.Shared.Next(50,55), cells);

        return new GridContext
        {
            Cells = cells
        };

    }
    public GridContext CreateHardGrid()
    {
        CreateRandomGrid();

        CheckFunctions.RemoveKDigits(Random.Shared.Next(56, 60), cells);

        return new GridContext
        {
            Cells = cells
        };

    }


}