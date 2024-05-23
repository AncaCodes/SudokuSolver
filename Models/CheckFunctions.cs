using System.Reflection.Metadata;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Components.Forms;

namespace SudokuSolver.Web.Models
{
    public static class CheckFunctions
    {

        public static void FillDiagonal(int[,] cells)
        {

            for (int i = 0; i < Settings.Dimension; i = i + Settings.SRootDimension)

                FillBox(i, i, cells);
        }

        public static bool UnusedInBox(int rowStart, int colStart, int num, int[,] cells)
        {
            for (int i = 0; i < Settings.SRootDimension; i++)
                for (int j = 0; j < Settings.SRootDimension; j++)
                    if (cells[rowStart + i, colStart + j] == num)
                        return false;

            return true;
        }

        public static void FillBox(int row, int col, int[,] cells)
        {

            int num;
            for (int i = 0; i < Settings.SRootDimension; i++)
            {
                for (int j = 0; j < Settings.SRootDimension; j++)
                {
                    do
                    {
                        num = Random.Shared.Next(Settings.MinCellValue, Settings.MaxCellValue);
                    }
                    while (!UnusedInBox(row, col, num, cells));

                    cells[row + i, col + j] = num;
                }
            }
        }

        public static void RemoveKDigits(int K, int[,] cells)
        {
            int count = K;
            while (count != 0)
            {
                int cellId = Random.Shared.Next(0, (Settings.Dimension * Settings.Dimension) - 1);

                int i = (cellId / Settings.Dimension);
                int j = cellId % Settings.Dimension;

                if (cells[i, j] != 0)
                {
                    count--;
                    cells[i, j] = 0;
                }
            }
        }

        public static bool CheckIfSafe(int i, int j, int num, int[,] cells)
        {
            return (UnusedInRow(i, num, cells) &&
                    UnusedInCol(j, num, cells) &&
                    UnusedInBox(i - i % Settings.SRootDimension, j - j % Settings.SRootDimension, num, cells));
        }

        public static bool UnusedInRow(int i, int num, int[,] cells)
        {
            for (int j = 0; j < Settings.Dimension; j++)
                if (cells[i, j] == num)
                    return false;
            return true;
        }

        public static bool UnusedInCol(int j, int num, int[,] cells)
        {
            for (int i = 0; i < Settings.Dimension; i++)
                if (cells[i, j] == num)
                    return false;
            return true;
        }


        public static bool FillRemaining(int i, int j, int[,] cells)
        {
            if (j >= Settings.Dimension && i < Settings.Dimension - 1)
            {
                i = i + 1;
                j = 0;
            }
            if (i >= Settings.Dimension && j >= Settings.Dimension)
                return true;

            if (i < Settings.SRootDimension)
            {
                if (j < Settings.SRootDimension)
                    j = Settings.SRootDimension;
            }
            else if (i < Settings.Dimension - Settings.SRootDimension)
            {
                if (j == (int)(i / Settings.SRootDimension) * Settings.SRootDimension)
                    j = j + Settings.SRootDimension;
            }
            else
            {
                if (j == Settings.Dimension - Settings.SRootDimension)
                {
                    i++;
                    j = 0;
                    if (i >= Settings.Dimension)
                        return true;
                }
            }

            for (int num = 1; num <= Settings.Dimension; num++)
            {
                if (CheckIfSafe(i, j, num, cells))
                {
                    cells[i, j] = num;
                    if (FillRemaining(i, j + 1, cells))
                        return true;

                    cells[i, j] = 0;
                }
            }
            return false;
        }



        // Function to solve the Sudoku
        public static bool SolveSudokuBacktracking(int[,] cells)
        {
            int row = 0, col = 0;
            if (!FindUnassignedLocation(cells, ref row, ref col))
                return true; // success!

            for (int num = 1; num <= Settings.Dimmension; num++)
            {
                if (CheckIfSafe(row, col, num, cells))
                {
                    cells[row, col] = num;

                    if (SolveSudokuBacktracking(cells))
                        return true;

                    cells[row, col] = 0;
                }
            }
            return false; // triggers backtracking
        }

        // Function to find an unassigned location
        private static bool FindUnassignedLocation(int[,] cells, ref int row, ref int col)
        {
            for (row = 0; row < Settings.Dimmension; row++)
            {
                for (col = 0; col < Settings.Dimmension; col++)
                {
                    if (cells[row, col] == 0)
                        return true;
                }
            }
            return false;
        }



        public static void SolveSudoku(int[,] Cells)
        {
            if (CheckFunctions.SolveSudokuBacktracking(Cells))
            {
                Print(Cells);
            }
            else
            {
                Console.WriteLine("No solution exists");
            }
        }

        static void Print(int[,] cells)
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Console.Write(cells[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}


