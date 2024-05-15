using System.Reflection.Metadata;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public static int ReturnIndices(int position)
        {
            return (position / Settings.Dimension) * 10 + (position % Settings.Dimension);

        }
        public static int FindZone(int i, int j)
        {
            if (i < 4)
            {
                if (j < 4)
                {
                    return 1;
                }
                else if (j < 7)
                {
                    return 2;
                }
                else return 3;
            }
            else if (i < 7)
            {
                if (j < 4)
                {
                    return 4;
                }
                else if (j < 7)
                {
                    return 5;
                }
                else return 6;
            }
            else
            {
                if (j < 4)
                {
                    return 7;
                }
                else if (j < 7)
                {
                    return 8;
                }
                else return 9;
            }
        }

        public static int CountPossibleValues(int i, int j, int[,,] c) //returns ijk(the only possible position and value or 0
        {
            int PossibleValues = 0;
            int CellValue = 0;
            for (int k = 1; k <= Settings.Dimension; k++)
            {

                if (c[i, j, k] == 1)
                {
                    PossibleValues++;
                    CellValue = k;
                }
            }
            if (PossibleValues == 1)
            {
                return i * 100 + j * 10 + CellValue;
            }
            return 0;

        }

        public static void ZoneRestriction(int[,,] c, int[,] Cells)
        {
            List<int> row = new List<int>();
            List<int> col = new List<int>();
            for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
            {
                Console.WriteLine("Test digit " + tested_digit);
                for (int rowstart = 1; rowstart < Settings.Dimension; rowstart = rowstart + 3)
                {
                    for (int colstart = 1; colstart < Settings.Dimension; colstart = colstart + 3)
                    {
                        Console.WriteLine("line: " + rowstart + "col: " + colstart);
                        if (UnusedInBox(rowstart, colstart, tested_digit, Cells))
                        {
                            for (int i = 0; i < Settings.SRootDimension; i++)
                            {
                                for (int j = 0; j < Settings.SRootDimension; j++)
                                {
                                    if (c[rowstart + i, colstart + j, tested_digit] == 1)
                                    {
                                        row.Add(rowstart + i);
                                        col.Add(colstart + j);

                                    }
                                }
                            }
                            //if i are identic...the only possible line in zone
                            if (row.First() == row.Last())
                            {
                                for (int index = 1; index <= Settings.Dimension; index++)
                                {
                                    if (!col.Contains(index))
                                    {
                                        c[row[0], index, tested_digit] = 0;
                                    }
                                }
                            }
                            else
                            {
                                col.Sort();
                                if (col.First() == col.Last())
                                {
                                    for (int index = 1; index <= Settings.Dimension; index++)
                                    {
                                        if (!row.Contains(index))
                                        {
                                            c[index, col[0], tested_digit] = 0;
                                        }
                                    }
                                }

                            }

                            row.Clear();
                            col.Clear();
                        }
                    }

                }
            }
        } 
        public static void SolveSudoku(int[,] Cells)
        {

            int[,,] c = new int[11, 11, 11];
            int[] line = new int[11];
            int[] col = new int[11];
            int[] zone = new int[11];

            int CellValue = 0, RelativePosition = 0, PossibleValues = 0;
            bool Solved = false;
            Console.WriteLine("works");

            //Initial transitions imposed by restrictions
            for (int i = 1; i <= Settings.Dimension; i++)
            {
                for (int j = 1; j <= Settings.Dimension; j++)
                {
                    CellValue = Cells[i - 1, j - 1];


                    if (CellValue != 0)
                    {
                        c[i, j, 10] = CellValue;//solution
                    }
                    else
                    {
                        for (int k = 1; k <= Settings.Dimension; k++)
                        {
                            c[i, j, k] = 1;
                        }
                        c[i, j, 10] = 0;

                    }
                }
            }
            int steps = 20;
            do
            {
                for (var i = 1; i <= Settings.Dimension; i++)
                {
                    for (var j = 1; j <= Settings.Dimension; j++)
                    {
                        if (c[i, j, 10] == 0)
                        {
                            PossibleValues = CheckFunctions.CountPossibleValues(i, j, c);
                            Console.WriteLine("Possible values: " + (i) + (j) + " are " + PossibleValues / 10);
                            if (PossibleValues != 0)
                            {
                                Console.WriteLine("The only value: " + (i) + (j) + " is " + PossibleValues / 10);
                                c[i, j, 10] = PossibleValues % 10;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    c[i, j, k] = 0;
                                }
                                break;
                            }
                        }
                    }
                }
                //update line, column, zone restriction
                CheckFunctions.ZoneRestriction(c,Cells);
                for (int index = 1; index <= Settings.Dimension; index++)
                {
                    for (int j = 1; j <= Settings.Dimension; j++)
                    {
                        if (c[index, j, 10] == 0)
                        {

                            PossibleValues = CheckFunctions.CountPossibleValues(index, j, c);
                            if (PossibleValues != 0)
                            {
                                Console.WriteLine("The only value: " + (index) + (j) + " is " + PossibleValues % 10);
                                RelativePosition = PossibleValues / 10 % 10; ;
                                c[index, RelativePosition, 10] = PossibleValues % 10;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    c[index, RelativePosition, k] = 0;
                                }
                                break;
                            }
                        }

                    }
                    for (int i = 1; i <= Settings.Dimension; i++)
                    {
                        if (c[RelativePosition, index, 10] == 0)
                        {
                            PossibleValues = CheckFunctions.CountPossibleValues(i, index, c);

                            if (PossibleValues != 0)
                            {
                                Console.WriteLine("line: " + RelativePosition + " col: " + index + " value " + PossibleValues % 10);
                                RelativePosition = PossibleValues / 100;
                                c[RelativePosition, index, 10] = PossibleValues % 10;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    c[RelativePosition, index, k] = 0;
                                }
                                break;
                            }
                        }
                    }
                }
                //Ckeck zones

                //special tests if no solution found
                steps--;
            } while (steps != 0);// while (!Solved);


        }

    }

}