/*using System.Reflection.Metadata;
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
                return (i * 100 + j * 10 + CellValue);
            }
            return 0;

        }
        public static void UpdateLines(int[,,] c, int[,] Cells)
        {
            int options;
            int possible_j;
            for (int i = 1; i <= Settings.Dimension; i++)
            {
                for (int digit = 1; digit <= Settings.Dimension; digit++)
                {
                    options = 0;
                    possible_j = 0;
                    for (int j = 1; j <= Settings.Dimension; j++)
                    {
                        if (c[i, j, digit] == 1)
                        {
                            options++;
                            possible_j = j;
                        }
                    }
                    if (options == 1)
                    {
                        c[i, possible_j, 10] = digit;
                        Cells[i - 1, possible_j - 1] = digit;
                        Console.WriteLine("*****Update line. Line" + i + "col " + possible_j + " digit " + digit);
                        for (int j = 1; j <= Settings.Dimension; j++)
                        {
                            c[i, j, digit] = 0;
                        }
                    }
                }
            }
            Console.WriteLine("**Update line ended");
        }

        public static void UpdateColums(int[,,] c, int[,] Cells)
        {
            int options;
            int possible_i;
            for (int j = 1; j <= Settings.Dimension; j++)
            {
                for (int digit = 1; digit <= Settings.Dimension; digit++)
                {
                    options = 0;
                    possible_i = 0;
                    for (int i = 1; i <= Settings.Dimension; i++)
                    {
                        if (c[i, j, digit] == 1)
                        {
                            options++;
                            possible_i = i;
                            Console.WriteLine("UpdateCoulumns line" + i + " col:" + j + " can be " + digit);
                        }
                    }
                    if (options == 1)
                    {
                        c[possible_i, j, 10] = digit;
                        Cells[possible_i - 1, j - 1] = digit;
                        Console.WriteLine("Update Col. Line" + possible_i + "col " + j + " digit " + digit);
                        for (int i = 1; i <= Settings.Dimension; i++)
                        {
                            c[i, j, digit] = 0;
                        }
                    }
                }
            }
        }

        public static void UpdateZones(int[,,] c, int[,] Cells)
        {
            int options, possible_i, possible_j;
            for (int rowstart = 1; rowstart < Settings.Dimension - 1; rowstart = rowstart + 3)
            {
                for (int colstart = 1; colstart < Settings.Dimension - 1; colstart = colstart + 3)
                {
                    for (int digit = 1; digit <= Settings.Dimension; digit++)
                    {
                        options = 0;
                        possible_i = 0;
                        possible_j = 0;
                        for (int i = 0; i < Settings.SRootDimension; i++)
                        {
                            for (int j = 0; j < Settings.SRootDimension; j++)
                            {
                                if (c[rowstart + i, colstart + j, digit] == 1)
                                {
                                    options++;
                                    possible_i = rowstart + i;
                                    possible_j = colstart + j;
                                }
                            }
                        }
                        if (options == 1)
                        {
                            c[possible_i, possible_j, 10] = digit;
                            Cells[possible_i - 1, possible_j - 1] = digit;
                            Console.WriteLine("Update Zone. Line" + possible_i + "col " + possible_j + " digit " + digit);
                            for (int index = 1; index <= Settings.Dimension; index++)
                            {
                                c[index, possible_j, digit] = 0;
                                c[possible_i, index, digit] = 0;
                            }
                        }
                    }
                }
            }
        }

        public static void ZoneRestriction(int[,,] c, int[,] Cells)
        {
            List<int> row = new List<int>();
            List<int> col = new List<int>();
            for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
            {
                for (int rowstart = 1; rowstart < Settings.Dimension; rowstart = rowstart + 3)
                {
                    for (int colstart = 1; colstart < Settings.Dimension; colstart = colstart + 3)
                    {
                        if (UnusedInBox(rowstart - 1, colstart - 1, tested_digit, Cells))
                        {
                            for (int i = 0; i < Settings.SRootDimension; i++)
                            {
                                for (int j = 0; j < Settings.SRootDimension; j++)
                                {
                                    if (rowstart + i < c.GetLength(0) && colstart + j < c.GetLength(1) && c[rowstart + i, colstart + j, tested_digit] == 1)
                                    {
                                        row.Add(rowstart + i);
                                        col.Add(colstart + j);
                                    }
                                }
                            }
                            if (row.Count > 0 && row.All(x => x == row[0]))
                            {
                                for (int index = 1; index <= Settings.Dimension; index++)
                                {
                                    if (!col.Contains(index))
                                    {
                                        c[row[0], index, tested_digit] = 0;
                                        Console.WriteLine("i are identic, row " + row[0] + "col  " + index + "   NOT: " + tested_digit);
                                    }
                                }
                            }
                            else if (col.Count > 0 && col.All(x => x == col[0]))
                            {
                                for (int index = 1; index <= Settings.Dimension; index++)
                                {
                                    if (!row.Contains(index))
                                    {
                                        c[index, col[0], tested_digit] = 0;
                                        Console.WriteLine("else row" + index + " col: " + col[0] + "      NOT: " + tested_digit);
                                    }
                                }
                            }

                            row.Clear();
                            col.Clear();
                        }
                    }
                }
            }
            Console.WriteLine("ZoneRestriction was executed.");
        }


        public static void LineColumnToBoxRestriction(int[,,] c, int[,] Cells)
        {
            int row_sum = 0, col_sum = 0, zone_sum = 0;
            for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
            {
                for (int rowstart = 1; rowstart < Settings.Dimension - 1; rowstart = rowstart + 3)
                {
                    for (int colstart = 1; colstart < Settings.Dimension - 1; colstart = colstart + 3)
                    {
                        for (int i = 0; i < Settings.SRootDimension; i++)
                        {
                            if (UnusedInRow(rowstart + i - 1, tested_digit, Cells))
                            {
                                zone_sum = 0;
                                row_sum = 0;
                                for (int j = 0; j < Settings.SRootDimension; j++)
                                {
                                    zone_sum += c[rowstart + i, colstart + j, tested_digit];
                                }
                                for (int j = 1; j <= Settings.Dimension; j++)
                                {
                                    row_sum += c[rowstart + i, j, tested_digit];
                                }
                                if (zone_sum == row_sum && zone_sum != 0)
                                {
                                    for (int index = rowstart; index < rowstart + Settings.SRootDimension - 1; index++)
                                    {
                                        for (int j = 0; j < Settings.SRootDimension; j++)
                                        {
                                            if (index != rowstart + i)
                                            {
                                                c[index, colstart + j, tested_digit] = 0;
                                                Console.WriteLine("Linetocol unused in row " +index+ " col "+colstart+j + " cannot be " + tested_digit);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (int j = 0; j < Settings.SRootDimension; j++)
                        {
                            if (UnusedInCol(colstart + j - 1, tested_digit, Cells))
                            {
                                zone_sum = 0;
                                col_sum = 0;
                                for (int i = 0; i < Settings.SRootDimension; i++)
                                {
                                    zone_sum += c[rowstart + i, colstart + j, tested_digit];
                                }
                                for (int i = 1; i <= Settings.Dimension; i++)
                                {
                                    col_sum += c[i, colstart + j, tested_digit];
                                }
                                if (zone_sum == col_sum && zone_sum != 0)
                                {
                                    for (int index = colstart; index < colstart + Settings.SRootDimension - 1; index++)
                                    {
                                        for (int i = 0; i < Settings.SRootDimension; i++)
                                        {
                                            if (index != colstart + i)
                                            {
                                                c[rowstart + i, index, tested_digit] = 0;
                                                Console.WriteLine("Linetocol unused in row "+rowstart +i+ " col: " +index + " cannot be " + tested_digit);
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

        public static void LineNakedPairsRestrictiction(int[,,] c, int[,] Cells)
        {
            int sum_digit1;
            int sum_digit2;
            List<int> col1 = new List<int>();
            List<int> col2 = new List<int>();
            for (int digit1 = 1; digit1 <= Settings.Dimension; digit1++)
            {
                for (int digit2 = 1; digit2 <= Settings.Dimension; digit2++)

                {
                    for (int i = 1; i <= Settings.Dimension; i++)
                    {

                        if (UnusedInRow(i - 1, digit1, Cells) && UnusedInRow(i - 1, digit2, Cells))//////////////////////
                        {
                            sum_digit1 = 0;
                            sum_digit2 = 0;
                            for (int j = 1; j <= Settings.Dimension; j++)
                            {
                                if (c[i, j, digit1] == 1)
                                {
                                    sum_digit1 += c[i, j, digit1];
                                    col1.Add(j);
                                }
                                if (c[i, j, digit2] == 1)
                                {
                                    sum_digit2 += c[i, j, digit2];
                                    col2.Add(j);
                                }
                            }
                            // (digit1, digit2) -  naked pair along line i
                            // Imposes restrictions over line i except (i, jnaked) where pairs appear
                            if ((sum_digit1 == sum_digit2 && sum_digit1 == 2) && (Enumerable.SequenceEqual(col1, col2)))
                            {
                                for (int j = 1; j <= Settings.Dimension; j++)
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
            Console.WriteLine("LineNakedPairsRestrictiction was executed.");
        }

        public static void ColNakedPairsRestrictiction(int[,,] c, int[,] Cells)
        {
            int sum_digit1 = 0;
            int sum_digit2 = 0;
            List<int> line1 = new List<int>();
            List<int> line2 = new List<int>();
            for (int digit1 = 1; digit1 <= Settings.Dimension; digit1++)
            {
                for (int digit2 = 1; digit2 <= Settings.Dimension; digit2++)

                {
                    for (int j = 1; j <= Settings.Dimension; j++)
                    {
                        sum_digit1 = 0;
                        sum_digit2 = 0;
                        for (int i = 0; i <= Settings.Dimension; i++)
                        {
                            if (c[i, j, 10] != 0)
                            {
                                break;
                            }
                            if (c[i, j, digit1] == 1)
                            {
                                sum_digit1 += c[i, j, digit1];
                                line1.Add(i);
                            }
                            if (c[i, j, digit2] == 1)
                            {
                                sum_digit2 += c[i, j, digit2];
                                line2.Add(i);
                            }
                        }
                        // (digit1, digit2) -  naked pair along column i
                        // Imposes restrictions over line i except (inaked, j) where pairs appear
                        if ((sum_digit1 == sum_digit2 && sum_digit1 == 2) && (Enumerable.SequenceEqual(line1, line2)))
                        {
                            for (int i = 1; i <= Settings.Dimension; i++)
                            {
                                if (!line1.Contains(i))
                                {
                                    c[i, j, digit1] = 0;
                                    c[i, j, digit2] = 0;
                                }
                            }
                        }
                        line1.Clear();
                        line2.Clear();
                    }
                }
            }
        }

        public static void ZoneNakedPairsRestrictiction(int[,,] c, int[,] Cells)
        {
            int posibilities;
            int total_nakedcells;
            int inaked, jnaked;
            List<int> indices = new List<int>();

            for (int rowstart = 1; rowstart < Settings.Dimension; rowstart = rowstart + 3)
            {
                for (int colstart = 1; colstart < Settings.Dimension; colstart = colstart + 3)
                {

                    for (int digit1 = 1; digit1 <= 9; digit1++)
                    {
                        if (UnusedInBox(rowstart - 1, colstart - 1, digit1, Cells))
                        {

                            for (int digit2 = 1; digit2 <= 9; digit2++)
                            {
                                if (UnusedInBox(rowstart - 1, colstart, digit2, Cells))
                                {
                                    posibilities = 0;
                                    total_nakedcells = 0;
                                    indices.Clear();
                                    for (int i = 0; i < Settings.SRootDimension; i++)
                                    {
                                        for (int j = 0; j < Settings.SRootDimension; j++)
                                        {
                                            if (c[rowstart + i, colstart + j, digit1] == 1 && c[rowstart + i, colstart + j, digit2] == 1)
                                            {
                                                posibilities++;
                                                for (int other_digit = 0; other_digit <= 9; other_digit++)
                                                {
                                                    posibilities += c[rowstart + i, colstart + j, other_digit];
                                                }
                                                if (posibilities == 2)
                                                {
                                                    indices.Add((rowstart + i) * 10 + colstart + j);
                                                    total_nakedcells++;
                                                }
                                            }
                                        }
                                    }

                                    if (total_nakedcells == 2 && indices.Any())
                                    {
                                        for (int i = 0; i < Settings.SRootDimension; i++)
                                        {
                                            for (int j = 0; j < Settings.SRootDimension; j++)
                                            {
                                                c[i, j, digit1] = 0;
                                                c[i, j, digit2] = 0;
                                            }
                                        }
                                        inaked = indices.First() / 10;
                                        jnaked = indices.First() % 10;
                                        c[inaked, jnaked, digit1] = 1;
                                        c[inaked, jnaked, digit2] = 1;
                                        inaked = indices.Last() / 10;
                                        jnaked = indices.Last() % 10;
                                        c[inaked, jnaked, digit1] = 1;
                                        c[inaked, jnaked, digit2] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void XWingRestriction(int[,,] c, int[,] Cells)
        {
            int possibilities1, possibilities2, j1, j2;
            List<int> indices_j = new List<int>();
            for (int digit = 1; digit <= 9; digit++)
            {
                for (int i1 = 1; i1 <= 9; i1++)
                {
                    if (UnusedInRow(i1 - 1, digit, Cells))
                    {
                        for (int i2 = 1; i2 <= 9; i2++)
                        {
                            if (UnusedInRow(i2 - 1, digit, Cells))
                            {
                                possibilities1 = 0;
                                possibilities2 = 0;
                                indices_j.Clear();
                                for (int j = 1; j <= 9; j++)
                                {
                                    possibilities1 += c[i1, j, digit];
                                    possibilities2 += c[i2, j, digit];
                                    if (c[i1, j, digit] == 1 && c[i2, j, digit] == 1)
                                    {
                                        indices_j.Add(j);
                                    }
                                }
                                if (possibilities1 == 2 && possibilities2 == 2)
                                {
                                    j1 = indices_j.FirstOrDefault();
                                    j2 = indices_j.LastOrDefault();
                                    for (int i = 1; i <= 9; i++)
                                    {
                                        c[i, j1, digit] = 0;
                                        c[i, j2, digit] = 0;
                                    }
                                    c[i1, j1, digit] = 1;
                                    c[i1, j2, digit] = 1;
                                    c[i2, j1, digit] = 1;
                                    c[i2, j2, digit] = 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void YWingRestriction(int[,,] c, int dimension)
        {
            // Step 1: Identify cells with exactly two c
            List<int> cellsWithTwoCandidates = new List<int>();

            for (int i = 1; i <= dimension; i++)
            {
                for (int j = 1; j <= dimension; j++)
                {
                    List<int> candidateList = new List<int>();
                    for (int digit = 1; digit <= dimension; digit++)
                    {
                        if (c[i, j, digit] == 1)
                        {
                            candidateList.Add(digit);
                        }
                    }
                    if (candidateList.Count == 2)
                    {
                        cellsWithTwoCandidates.Add((i - 1) * dimension + j);
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
                        int p1 = cellsWithTwoCandidates[l1];
                        int p2 = cellsWithTwoCandidates[l2];
                        int p3 = cellsWithTwoCandidates[l3];

                        int i1 = (p1 - 1) / dimension + 1;
                        int j1 = (p1 - 1) % dimension + 1;
                        int i2 = (p2 - 1) / dimension + 1;
                        int j2 = (p2 - 1) % dimension + 1;
                        int i3 = (p3 - 1) / dimension + 1;
                        int j3 = (p3 - 1) % dimension + 1;

                        List<int> candidates1 = GetCandidates(c, i1, j1, dimension);
                        List<int> candidates2 = GetCandidates(c, i2, j2, dimension);
                        List<int> candidates3 = GetCandidates(c, i3, j3, dimension);

                        // Step 3: Check if they form a Y-Wing with p1 as pivot
                        if (FormsYWing(candidates1, candidates2, candidates3, out int restrictedDigit))
                        {
                            // Step 4: Apply restrictions according to Y-Wing
                            ApplyYWingRestrictions(c, dimension, i1, j1, i2, j2, i3, j3, restrictedDigit);
                        }
                    }
                }
            }
        }

        private static List<int> GetCandidates(int[,,] c, int i, int j, int dimension)
        {
            List<int> candidateList = new List<int>();
            for (int digit = 1; digit <= dimension; digit++)
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
                    if (candidates1.Contains(digit) && candidates2.Contains(digit) && !candidates3.Contains(digit) ||
                        candidates1.Contains(digit) && candidates3.Contains(digit) && !candidates2.Contains(digit) ||
                        candidates2.Contains(digit) && candidates3.Contains(digit) && !candidates1.Contains(digit))
                    {
                        restrictedDigit = digit;
                        return true;
                    }
                }
            }
            return false;
        }

        private static void ApplyYWingRestrictions(int[,,] c, int dimension, int i1, int j1, int i2, int j2, int i3, int j3, int restrictedDigit)
        {
            for (int i = 1; i <= dimension; i++)
            {
                for (int j = 1; j <= dimension; j++)
                {
                    if ((i != i1 || j != j1) && (i != i2 || j != j2) && (i != i3 || j != j3) &&
                        (IsCellSeenBy(i, j, i1, j1) && IsCellSeenBy(i, j, i2, j2) || IsCellSeenBy(i, j, i1, j1) && IsCellSeenBy(i, j, i3, j3) || IsCellSeenBy(i, j, i2, j2) && IsCellSeenBy(i, j, i3, j3)))
                    {
                        c[i, j, restrictedDigit] = 0;
                    }
                }
            }
        }

        private static bool IsCellSeenBy(int i1, int j1, int i2, int j2)
        {
            return i1 == i2 || j1 == j2 || (i1 - 1) / 3 == (i2 - 1) / 3 && (j1 - 1) / 3 == (j2 - 1) / 3;
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
                    Console.WriteLine  ("Cell value:" + CellValue);

                    if (CellValue != 0)
                    {
                        c[i, j, 10] = CellValue;//solution
                        for (int k = 1; k <= Settings.Dimension; k++)
                        {
                            c[i, j, k] = 0;
                        }
                        Console.WriteLine("C[" + i+"," + j + ",10] is " + c[i, j, 10]);
                    }
                    else
                    {
                        for (int k = 1; k <= Settings.Dimension; k++)
                        {
                            c[i, j, k] = 1;
                        }
                        c[i, j, 10] = 0;
                        Console.WriteLine("All k are 1. C[" + i+"," + j + ",10] is " + c[i, j, 10]);

                    }
                }
            }

            int steps = 50;
            do
            {
                for (var i = 1; i <= Settings.Dimension; i++)
                {
                    for (var j = 1; j <= Settings.Dimension; j++)
                    {
                        if (c[i, j, 10] == 0)
                        {
                            PossibleValues = CheckFunctions.CountPossibleValues(i, j, c);
                            if (PossibleValues != 0)
                            {
                                Console.WriteLine("The only value: " + i + j + " is " + PossibleValues % 10);
                                c[i, j, 10] = PossibleValues % 10;
                                Cells[i - 1, j - 1] = PossibleValues % 10;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    c[i, j, k] = 0;
                                }
                            }
                        }
                    }
                }
                //update line, column, zone restriction
                CheckFunctions.LineColumnToBoxRestriction(c, Cells);
                CheckFunctions.ZoneRestriction(c, Cells);
                CheckFunctions.LineNakedPairsRestrictiction(c, Cells);
                CheckFunctions.ColNakedPairsRestrictiction(c, Cells);
                //CheckFunctions.XWingRestriction(c, Cells); 
                //CheckFunctions.YWingRestriction(c,Settings.Dimension);
                for (int index = 1; index <= Settings.Dimension; index++)
                {
                    for (int j = 1; j <= Settings.Dimension; j++)
                    {
                        if (c[index, j, 10] == 0)
                        {

                            PossibleValues = CheckFunctions.CountPossibleValues(index, j, c);
                            if (PossibleValues != 0)
                            {
                                RelativePosition = PossibleValues / 10 % 10;
                                Console.WriteLine("The only value: " + index + RelativePosition + " is " + PossibleValues % 10);
                                c[index, RelativePosition, 10] = PossibleValues % 10;
                                Cells[index - 1, RelativePosition - 1] = PossibleValues % 10;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    c[index, RelativePosition, k] = 0;
                                }
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
                                RelativePosition = PossibleValues / 100;
                                Console.WriteLine("Main: line " + RelativePosition + " col: " + index + " set value " + PossibleValues % 10);
                                c[RelativePosition, index, 10] = PossibleValues % 10;
                                Cells[RelativePosition - 1, index - 1] = PossibleValues % 10;
                                for (int k = 1; k <= Settings.Dimension; k++)
                                {
                                    c[RelativePosition, index, k] = 0;
                                }
                                break;
                            }
                        }
                    }
                }
                CheckFunctions.UpdateLines(c, Cells);
                CheckFunctions.UpdateColums(c, Cells);
                CheckFunctions.UpdateZones(c, Cells);
                //Ckeck zones

                //special tests if no solution found
                steps--;
            } while (steps != 0);// while (!Solved);
            for (int i = 1; i <= Settings.Dimension; i++)
            {
                for (int j = 1; j <= Settings.Dimension; j += 3)
                {
                    Console.WriteLine(c[i, j, 10] + " " + c[i, j + 1, 10] + " " + c[i, j + 2, 10]);
                }
                Console.WriteLine();
            }

            for (int i = 0; i < Settings.Dimension; i++)
            {
                for (int j = 3; j < Settings.Dimension; j += 3)
                {
                    Console.WriteLine(Cells[i, j - 3] + " " + Cells[i, j - 2] + " " + Cells[i, j - 1]);

                }
                Console.WriteLine() ;
            }
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine("C[1,1,"+i+"]=" + c[1, 1, i]);
                Console.WriteLine("C[3,4," + i + " is " + c[3, 4, i]);

            }
            Console.WriteLine();

        }
    }

}

*/