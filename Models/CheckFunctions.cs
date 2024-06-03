//using System.Reflection.Metadata;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Components.Forms;
//using System.Runtime.CompilerServices;

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
                i++;
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
                    j += Settings.SRootDimension;
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


        private static int FindUnassignedLocation(int[,] cells, ref int row, ref int col)
        {
            for (row = 0; row < Settings.Dimension; row++)
            {
                for (col = 0; col < Settings.Dimension; col++)
                {
                    if (cells[row, col] == 0)
                    {
                        int position = row * 10 + col;
                        return position;
                    }
                }
            }
            return -1;
        }
        public static void Print(int[,] cells)
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

        public static void ZoneRestriction(int[,,] c, int[,] Cells)
        {
            List<int> row = new List<int>();
            List<int> col = new List<int>();
            for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
            {
                for (int rowstart = 0; rowstart < Settings.Dimension; rowstart += 3)
                {
                    for (int colstart = 0; colstart < Settings.Dimension; colstart += 3)
                    {
                        if (UnusedInBox(rowstart, colstart, tested_digit, Cells))
                        {
                            row.Clear();
                            col.Clear();
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
                            if (row.Count > 0 && row.All(x => x == row[0]))
                            {
                                for (int index = 0; index < Settings.Dimension; index++)
                                {
                                    if (!col.Contains(index))
                                    {
                                        c[row[0], index, tested_digit] = 0;
                                    }
                                }
                            }
                            else if (col.Count > 0 && col.All(x => x == col[0]))
                            {
                                for (int index = 0; index < Settings.Dimension; index++)
                                {
                                    if (!row.Contains(index))
                                    {
                                        c[index, col[0], tested_digit] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
            // Console.WriteLine("LineColumnToBoxRestriction was executed.");
        }

        public static void LineNakedPairsRestriction(int[,,] c, int[,] Cells)
        {
            int sum_digit1;
            int sum_digit2;
            List<int> col1 = new List<int>();
            List<int> col2 = new List<int>();
            for (int digit1 = 1; digit1 < Settings.Dimension; digit1++)
            {
                for (int digit2 = digit1 + 1; digit2 <= Settings.Dimension; digit2++)
                {
                    for (int i = 0; i < Settings.Dimension; i++)
                    {
                        if (digit1 != digit2)
                        {
                            if (UnusedInRow(i, digit1, Cells) && UnusedInRow(i, digit2, Cells))
                            {
                                sum_digit1 = 0;
                                sum_digit2 = 0;
                                for (int j = 0; j < Settings.Dimension; j++)
                                {
                                    if (c[i, j, digit1] == 1)
                                    {
                                        sum_digit1++;
                                        col1.Add(j);
                                    }
                                    if (c[i, j, digit2] == 1)
                                    {
                                        sum_digit2++;
                                        col2.Add(j);
                                    }
                                }

                                if (sum_digit1 == 2 && sum_digit2 == 2 && col1.SequenceEqual(col2))
                                {
                                    for (int j = 0; j < Settings.Dimension; j++)
                                    {
                                        if (!col1.Contains(j))
                                        {
                                            c[i, j, digit1] = 0;
                                            c[i, j, digit2] = 0;
                                        }
                                    }
                                }
                                col1.Clear();
                                col2.Clear();
                            }
                        }
                    }
                }
            }
            //Console.WriteLine("LineNakedPairsRestriction was executed.");
        }
        //public static void LineNakedTriplesRestriction(int[,,] c, int[,] Cells)
        //{
        //    List<int> candidates = new List<int>();
        //    for (int i = 0; i < Settings.Dimension; i++)
        //    {
        //        for (int j1 = 0; j1 < Settings.Dimension - 2; j1++)
        //        {
        //            for (int j2 = j1 + 1; j2 < Settings.Dimension - 1; j2++)
        //            {
        //                for (int j3 = j2 + 1; j3 < Settings.Dimension; j3++)
        //                {
        //                    candidates.Clear();
        //                    for (int digit = 1; digit <= Settings.Dimension; digit++)
        //                    {
        //                        if (c[i, j1, digit] == 1) candidates.Add(digit);
        //                        if (c[i, j2, digit] == 1 && !candidates.Contains(digit)) candidates.Add(digit);
        //                        if (c[i, j3, digit] == 1 && !candidates.Contains(digit)) candidates.Add(digit);
        //                    }

        //                    if (candidates.Count == 3)
        //                    {
        //                        for (int j = 0; j < Settings.Dimension; j++)
        //                        {
        //                            if (j != j1 && j != j2 && j != j3)
        //                            {
        //                                foreach (int digit in candidates)
        //                                {
        //                                    c[i, j, digit] = 0;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //Console.WriteLine("LineNakedTriplesRestriction was executed.");
        //}
        public static void ColNakedPairsRestriction(int[,,] c, int[,] cells)
        {
            for (int j = 0; j < Settings.Dimension; j++)
            {
                for (int digit1 = 1; digit1 < Settings.Dimension; digit1++)
                {
                    for (int digit2 = digit1 + 1; digit2 <= Settings.Dimension; digit2++)
                    {
                        if (digit1 != digit2)
                        {
                            List<int> line1 = new List<int>();
                            List<int> line2 = new List<int>();

                            for (int i = 0; i < Settings.Dimension; i++)
                            {
                                if (c[i, j, digit1] == 1)
                                {
                                    line1.Add(i);

                                }
                                if (c[i, j, digit2] == 1)
                                {
                                    line2.Add(i);
                                }
                            }

                            if (line1.Count == 2 && line1.SequenceEqual(line2))
                            {
                                for (int i = 0; i < Settings.Dimension; i++)
                                {
                                    if (!line1.Contains(i))
                                    {
                                        c[i, j, digit1] = 0;
                                        c[i, j, digit2] = 0;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            // Console.WriteLine("ColNakedPairsRestriction was executed.");
        }

        public static void XWingRestriction(int[,,] c, int[,] cells)
        {
            for (int digit = 1; digit <= Settings.Dimension; digit++)
            {
                for (int i1 = 0; i1 < Settings.Dimension; i1++)
                {
                    if (UnusedInRow(i1, digit, cells))
                    {
                        for (int i2 = i1 + 1; i2 < Settings.Dimension; i2++)
                        {
                            if (UnusedInRow(i2, digit, cells))
                            {
                                List<int> commonColumns = FindCommonColumns(c, i1, i2, digit);
                                if (commonColumns.Count == 2)
                                {
                                    ApplyXWingConstraint(c, cells, i1, i2, commonColumns, digit);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static List<int> FindCommonColumns(int[,,] c, int row1, int row2, int digit)
        {
            List<int> commonColumns = new List<int>();
            for (int j = 0; j < Settings.Dimension; j++)
            {
                if (c[row1, j, digit] == 1 && c[row2, j, digit] == 1)
                {
                    commonColumns.Add(j);
                }
            }
            return commonColumns;
        }

        private static void ApplyXWingConstraint(int[,,] c, int[,] cells, int row1, int row2, List<int> commonColumns, int digit)
        {
            int column1 = commonColumns[0];
            int column2 = commonColumns[1];
            for (int i = 0; i < Settings.Dimension; i++)
            {
                if (i != row1 && i != row2)
                {
                    c[i, column1, digit] = 0;
                    c[i, column2, digit] = 0;
                }
            }
        }

        public static void YWingRestriction(int[,,] c)
        {
            // Step 1: Identify cells with exactly two candidates
            List<(int, int)> cellsWithTwoCandidates = new List<(int, int)>();

            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    List<int> candidateList = GetCandidates(c, i, j);
                    if (candidateList.Count == 2)
                    {
                        cellsWithTwoCandidates.Add((i, j));
                    }
                }
            }

            // Step 2: For all possible triples of places (p(l1), p(l2), p(l3))
            for (int l1 = 0; l1 < cellsWithTwoCandidates.Count; l1++)
            {
                for (int l2 = l1 + 1; l2 < cellsWithTwoCandidates.Count; l2++)
                {
                    for (int l3 = l2 + 1; l3 < cellsWithTwoCandidates.Count; l3++)
                    {
                        (int i1, int j1) = cellsWithTwoCandidates[l1];
                        (int i2, int j2) = cellsWithTwoCandidates[l2];
                        (int i3, int j3) = cellsWithTwoCandidates[l3];

                        List<int> candidates1 = GetCandidates(c, i1, j1);
                        List<int> candidates2 = GetCandidates(c, i2, j2);
                        List<int> candidates3 = GetCandidates(c, i3, j3);

                        // Step 3: Check if they form a Y-Wing with p1 as pivot
                        if (FormsYWing(candidates1, candidates2, candidates3, out int restrictedDigit))
                        {
                            // Step 4: Apply restrictions according to Y-Wing
                            ApplyYWingRestrictions(c, i1, j1, i2, j2, i3, j3, restrictedDigit);
                        }
                    }
                }
            }
        }

        private static List<int> GetCandidates(int[,,] c, int i, int j)
        {
            List<int> candidateList = new List<int>();
            for (int digit = 1; digit <= Settings.Dimension; digit++)
            {
                if (c[i, j, digit] == 1)
                {
                    candidateList.Add(digit);
                }
            }
            return candidateList;
        }

        private static bool FormsYWing(List<int> candidates1, List<int> candidates2, List<int> candidates3, out int restrictedDigit)
        {
            restrictedDigit = -1;
            HashSet<int> allCandidates = new HashSet<int>(candidates1);
            allCandidates.UnionWith(candidates2);
            allCandidates.UnionWith(candidates3);

            if (allCandidates.Count == 3)
            {
                foreach (int digit in allCandidates)
                {
                    if ((candidates1.Contains(digit) && candidates2.Contains(digit) && !candidates3.Contains(digit)) ||
                        (candidates1.Contains(digit) && candidates3.Contains(digit) && !candidates2.Contains(digit)) ||
                        (candidates2.Contains(digit) && candidates3.Contains(digit) && !candidates1.Contains(digit)))
                    {
                        restrictedDigit = digit;
                        return true;
                    }
                }
            }
            return false;
        }

        private static void ApplyYWingRestrictions(int[,,] c, int i1, int j1, int i2, int j2, int i3, int j3, int restrictedDigit)
        {
            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    if ((i != i1 || j != j1) && (i != i2 || j != j2) && (i != i3 || j != j3) &&
                        ((IsCellSeenBy(i, j, i1, j1) && IsCellSeenBy(i, j, i2, j2)) ||
                        (IsCellSeenBy(i, j, i1, j1) && IsCellSeenBy(i, j, i3, j3)) ||
                        (IsCellSeenBy(i, j, i2, j2) && IsCellSeenBy(i, j, i3, j3))))
                    {
                        c[i, j, restrictedDigit] = 0;
                    }
                }
            }
        }

        private static bool IsCellSeenBy(int i1, int j1, int i2, int j2)
        {
            return i1 == i2 || j1 == j2 || (i1 / 3 == i2 / 3 && j1 / 3 == j2 / 3);
        }

        public static int[,,] InitializePossibilities(int[,] cells)
        {
            int[,,] possibilities = new int[Settings.Dimension, Settings.Dimension, Settings.Dimension + 2];
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
                    else
                    {
                        possibilities[i, j, 10] = cells[i, j];
                    }
                }
            }
            UpdatePossibilities(possibilities);
            return possibilities;
        }

        private static void ApplySpecialRestrictions(int[,,] possibilities, int[,] cells)
        {
            LineColumnToBoxRestriction(possibilities, cells);
            ZoneRestriction(possibilities, cells);
            LineNakedPairsRestriction(possibilities, cells);
            //LineNakedTriplesRestriction(possibilities, cells);
            ColNakedPairsRestriction(possibilities, cells);
            //XWingRestriction(possibilities, cells);
            //YWingRestriction(possibilities);
        }

        private static int CountPossibilities(int[,,] possibilities, int row, int col)
        {
            int count = 0;
            for (int k = 1; k <= Settings.Dimension; k++)
            {
                if (possibilities[row, col, k] == 1)
                {
                    count++;
                }
            }
            return count;
        }

        private static int FindLastPossibility(int[,,] possibilities, int row, int col)
        {
            for (int k = 1; k <= Settings.Dimension; k++)
            {
                if (possibilities[row, col, k] == 1)
                {
                    return k;
                }
            }
            return -1;
        }

        private static bool HasUnsolvedCells(int[,] cells)
        {
            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    if (cells[i, j] == 0)
                        return true;
                }
            }
            return false;
        }

        private static bool ApplyLineExclusivity(int[,,] possibilities, int[,] cells)
        {
            bool progress = false;
            for (int digit = 1; digit <= 9; digit++)
            {

                for (int i = 0; i < Settings.Dimension; i++)
                {
                    int sum = 0;
                    int lastCol = -1;
                    for (int j = 0; j < Settings.Dimension; j++)
                    {
                        if (possibilities[i, j, digit] == 1)
                        {
                            sum++;
                            lastCol = j;
                            if (sum >= 2)
                            {
                                break;
                            }
                        }
                    }

                    if (sum == 1)
                    {
                        if (cells[i, lastCol] == 0)
                        {
                            if (CheckIfSafe(i, lastCol, digit, cells))
                            {
                                cells[i, lastCol] = digit;
                                possibilities[i, lastCol, 10] = digit;
                                progress = true;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    possibilities[i, lastCol, k] = 0;
                                }
                            }
                        }
                    }
                }
            }
            return progress;
        }

        private static bool ApplyColumnExclusivity(int[,,] possibilities, int[,] cells)
        {

            bool progress = false;
            for (int digit = 1; digit <= 9; digit++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    int sum = 0;
                    int lastRow = -1;
                    for (int i = 0; i < Settings.Dimension; i++)
                    {
                        if (possibilities[i, j, digit] == 1)
                        {
                            sum++;
                            lastRow = i;
                            if (sum >= 2)
                            {
                                break;
                            }
                        }
                    }

                    if (sum == 1 && cells[lastRow, j] == 0)
                    {
                        if (CheckIfSafe(lastRow, j, digit, cells))
                        {
                            cells[lastRow, j] = digit;
                            possibilities[lastRow, j, 10] = digit;
                            progress = true;
                            for (int k = 1; k <= Settings.Dimension; k++)
                            {
                                possibilities[lastRow, j, k] = 0;
                            }
                        }
                    }
                }
            }
            return progress;
        }

        private static bool ApplyZoneExclusivity(int[,,] possibilities, int[,] cells)
        {
            bool progress = false;
            for (int digit = 1; digit <= 9; digit++)
            {
                for (int rowstart = 0; rowstart < Settings.Dimension - 2; rowstart += Settings.SRootDimension)
                {
                    for (int colstart = 0; colstart < Settings.Dimension - 2; colstart += Settings.SRootDimension)
                    {
                        int sum = 0;
                        int lastRow = -1;
                        int lastCol = -1;
                        for (int i = 0; i < Settings.SRootDimension; i++)
                        {
                            for (int j = 0; j < Settings.SRootDimension; j++)
                            {
                                if (possibilities[rowstart + i, colstart + j, digit] == 1)
                                {
                                    sum++;
                                    lastRow = rowstart + i;
                                    lastCol = colstart + j;
                                }
                            }
                        }

                        if (sum == 1)
                        {
                            if (cells[lastRow, lastCol] == 0 && CheckIfSafe(lastRow, lastCol, digit, cells))
                            {
                                cells[lastRow, lastCol] = digit;
                                possibilities[lastRow, lastCol, 10] = digit;
                                progress = true;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    possibilities[lastRow, lastCol, k] = 0;
                                }
                            }
                        }
                    }
                }
            }
            return progress;
        }

        public static bool FindCellValue(int[,,] possibilities, int[,] cells)
        {
            bool progress = false;
            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    if (cells[i, j] == 0)
                    {

                        int possibleCount = CountPossibilities(possibilities, i, j);
                        if (possibleCount == 1)
                        {
                            int digit = FindLastPossibility(possibilities, i, j);
                            if (CheckIfSafe(i, j, digit, cells))
                            {
                                cells[i, j] = digit;
                                possibilities[i, j, 10] = digit;
                                Console.WriteLine("Line " + i + " col " + j + " set as " + digit);
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    possibilities[i, j, k] = 0;
                                }
                                UpdatePossibilities(possibilities);
                                progress = true;
                            }
                        }
                    }
                }
            }
            return progress;
        }
        public static bool SolveBacktracking(int[,] cells)
        {
            int[,,] possibilities = InitializePossibilities(cells);
            UpdatePossibilities(possibilities);
            bool progress = true;
            int ok = 100;
            while (progress && HasUnsolvedCells(cells))
            {
                progress = false;
                while (FindCellValue(possibilities, cells))
                {
                    progress = (ApplyColumnExclusivity(possibilities, cells) || ApplyLineExclusivity(possibilities, cells) || ApplyZoneExclusivity(possibilities, cells));
                }
                progress = (ApplyColumnExclusivity(possibilities, cells) || ApplyLineExclusivity(possibilities, cells) || ApplyZoneExclusivity(possibilities, cells));

                if (!progress)
                {
                    Console.WriteLine("Apply SPECIAL");
                    ApplySpecialRestrictions(possibilities, cells);
                    progress = FindCellValue(possibilities, cells);
                    progress = (progress || ApplyColumnExclusivity(possibilities, cells) || ApplyLineExclusivity(possibilities, cells) || ApplyZoneExclusivity(possibilities, cells));
                }
                ok--;
                Console.WriteLine("OK is " + ok + " Progress is " + progress + " Hasunsolved: " + HasUnsolvedCells(cells));
            }
            if (HasUnsolvedCells(cells))
            {
                for (int i = 0; i < Settings.Dimension; i++)
                {
                    for (int j = 0; j < Settings.Dimension; j++)
                    {
                        if (cells[i, j] == 0)
                        {
                            for (int digit = 1; digit <= 9; digit++)
                            {
                                if (CheckIfSafe(i, j, digit, cells)){
                                    cells[i, j] = digit;
                                    if (SolveBacktracking(cells) == false)
                                    {
                                        cells[i, j] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    if (cells[i, j] == 0)
                    {
                        for (int digit = 1; digit <= Settings.Dimension; digit++)
                        {
                            if (possibilities[i, j, digit] == 1) Console.WriteLine(" Row: " + i + " Col: " + j + " Digit: " + digit + " can be " + possibilities[i, j, digit]);
                        }
                    }
                }
            }
            if (!HasUnsolvedCells(cells))
            {
                Print(cells);
                return true;
            }
            else return false;

        }



        public static void UpdatePossibilities(int[,,] possibilities)
        {
            int CellValue;
            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 0; j < Settings.Dimension; j++)
                {
                    if (possibilities[i, j, 10] != 0)
                    {
                        CellValue = possibilities[i, j, 10];
                        for (int index = 0; index < Settings.Dimension; index++)
                        {
                            possibilities[index, j, CellValue] = 0;
                            possibilities[i, index, CellValue] = 0;
                        }
                    }
                }
            }
            for (int rowStart = 0; rowStart < Settings.Dimension; rowStart += Settings.SRootDimension)
            {
                for (int colStart = 0; colStart < Settings.Dimension; colStart += Settings.SRootDimension)
                {
                    for (int i = rowStart; i < rowStart + Settings.SRootDimension; i++)
                    {
                        for (int j = colStart; j < colStart + Settings.SRootDimension; j++)
                        {
                            if (possibilities[i, j, 10] != 0)
                            {
                                CellValue = possibilities[i, j, 10];
                                for (int index_i = rowStart; index_i < rowStart + Settings.SRootDimension; index_i++)
                                {
                                    for (int index_j = colStart; index_j < colStart + Settings.SRootDimension; index_j++)
                                    {
                                        possibilities[index_i, index_j, CellValue] = 0;
                                    }
                                }

                            }
                        }
                    }
                }
            }

        }

    }
}
