using Microsoft.VisualBasic;
using SudokuSolver.Web.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;

namespace SudokuSolver.Web.Services;


public class SudokuGridFactory : ISudokuGridFactory
{

    public GridContext CreateRandomGrid()
    {
        int[,] cells = new int[Settings.Dimension, Settings.Dimension];

        CheckFunctions.fillDiagonal(cells);

        CheckFunctions.fillRemaining(0, Settings.SRN, cells);

        return new GridContext
        {
            Cells = cells
        };

    }

}