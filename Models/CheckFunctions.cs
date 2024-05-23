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


        private static bool FindUnassignedLocation(int[,] cells, ref int row, ref int col)
        {
            for (row = 0; row < Settings.Dimension; row++)
            {
                for (col = 0; col < Settings.Dimension; col++)
                {
                    if (cells[row, col] == 0)
                        return true;
                }
            }
            return false;
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

        public static bool SolveSudokuBacktracking(int[,] cells)
        {
            int[,,] possibilities = InitializePossibilities(cells);
            LineColumnToBoxRestriction(possibilities, cells);

            int row = 0, col = 0;
            if (!FindUnassignedLocation(cells, ref row, ref col))
                return true;

            for (int num = 1; num <= Settings.Dimension; num++)
            {
                if (CheckIfSafe(row, col, num, cells))
                {
                    cells[row, col] = num;
                    LineColumnToBoxRestriction(possibilities, cells);

                    if (SolveSudokuBacktracking(cells))
                        return true;

                    cells[row, col] = 0;
                }
            }
            return false;
        }

        public static int[,,] InitializePossibilities(int[,] cells)
        {
            int[,,] possibilities = new int[Settings.Dimension, Settings.Dimension, Settings.Dimension + 1];
            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    if (cells[i, j] == 0)
                    {
                        for (int k = 1; k <= Settings.Dimension; k++)
                        {
                            possibilities[i, j, k] = 1;
                        }
                    }
                }
            }
            return possibilities;
        }

        public static void LineColumnToBoxRestriction(int[,,] c, int[,] Cells)
        {
            int row_sum = 0, col_sum = 0, zone_sum = 0;
            for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
            {
                for (int rowstart = 0; rowstart < Settings.Dimension; rowstart += 3)
                {
                    for (int colstart = 0; colstart < Settings.Dimension; colstart += 3)
                    {
                        for (int i = 0; i < Settings.SRootDimension; i++)
                        {
                            if (UnusedInRow(rowstart + i, tested_digit, Cells))
                            {
                                zone_sum = 0;
                                row_sum = 0;
                                for (int j = 0; j < Settings.SRootDimension; j++)
                                {
                                    zone_sum += c[rowstart + i, colstart + j, tested_digit];
                                }
                                for (int j = 0; j < Settings.Dimension; j++)
                                {
                                    row_sum += c[rowstart + i, j, tested_digit];
                                }
                                if (zone_sum == row_sum && zone_sum != 0)
                                {
                                    for (int index = rowstart; index < rowstart + Settings.SRootDimension; index++)
                                    {
                                        for (int j = 0; j < Settings.SRootDimension; j++)
                                        {
                                            if (index != rowstart + i)
                                            {
                                                c[index, colstart + j, tested_digit] = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (int j = 0; j < Settings.SRootDimension; j++)
                        {
                            if (UnusedInCol(colstart + j, tested_digit, Cells))
                            {
                                zone_sum = 0;
                                col_sum = 0;
                                for (int i = 0; i < Settings.SRootDimension; i++)
                                {
                                    zone_sum += c[rowstart + i, colstart + j, tested_digit];
                                }
                                for (int i = 0; i < Settings.Dimension; i++)
                                {
                                    col_sum += c[i, colstart + j, tested_digit];
                                }
                                if (zone_sum == col_sum && zone_sum != 0)
                                {
                                    for (int index = colstart; index < colstart + Settings.SRootDimension; index++)
                                    {
                                        for (int i = 0; i < Settings.SRootDimension; i++)
                                        {
                                            if (index != colstart + j)
                                            {
                                                c[rowstart + i, index, tested_digit] = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("LineColumnToBoxRestriction was executed.");
        }

        public static void SolveSudoku(int[,] cells)
        {
            if (CheckFunctions.SolveSudokuBacktracking(cells))
            {
                Print(cells);
            }
            else
            {
                Console.WriteLine("No solution exists");
            }
        }
    }
}


