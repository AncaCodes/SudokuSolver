namespace SudokuSolver.Web.Models
{
    public static class CheckFunctions
    {


        public static void fillDiagonal(int[,] cells)
        {

            for (int i = 0; i < Settings.Dimension; i = i + Settings.SRN)

                fillBox(i, i, cells);
        }

        public static bool unUsedInBox(int rowStart, int colStart, int num, int[,] cells)
        {
            for (int i = 0; i < Settings.SRN; i++)
                for (int j = 0; j < Settings.SRN; j++)
                    if (cells[rowStart + i, colStart + j] == num)
                        return false;

            return true;
        }

        public static void fillBox(int row, int col, int[,] cells)
        {

            int num;
            for (int i = 0; i < Settings.SRN; i++)
            {
                for (int j = 0; j < Settings.SRN; j++)
                {
                    do
                    {
                        num = Random.Shared.Next(Settings.MinCellValue, Settings.MaxCellValue);
                    }
                    while (!unUsedInBox(row, col, num, cells));

                    cells[row + i, col + j] = num;
                }
            }
        }


        public static bool CheckIfSafe(int i, int j, int num, int[,] cells)
        {
            return (unUsedInRow(i, num, cells) &&
                    unUsedInCol(j, num, cells) &&
                    unUsedInBox(i - i % Settings.SRN, j - j % Settings.SRN, num, cells));
        }

        public static bool unUsedInRow(int i, int num, int[,] cells)
        {
            for (int j = 0; j < Settings.Dimension; j++)
                if (cells[i, j] == num)
                    return false;
            return true;
        }

        public static bool unUsedInCol(int j, int num, int[,] cells)
        {
            for (int i = 0; i < Settings.Dimension; i++)
                if (cells[i, j] == num)
                    return false;
            return true;
        }


        public static bool fillRemaining(int i, int j, int[,] cells)
        {
            if (j >= Settings.Dimension && i < Settings.Dimension - 1)
            {
                i = i + 1;
                j = 0;
            }
            if (i >= Settings.Dimension && j >= Settings.Dimension)
                return true;

            if (i < Settings.SRN)
            {
                if (j < Settings.SRN)
                    j = Settings.SRN;
            }
            else if (i < Settings.Dimension - Settings.SRN)
            {
                if (j == (int)(i / Settings.SRN) * Settings.SRN)
                    j = j + Settings.SRN;
            }
            else
            {
                if (j == Settings.Dimension - Settings.SRN)
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
                    if (fillRemaining(i, j + 1, cells))
                        return true;

                    cells[i, j] = 0;
                }
            }
            return false;
        }


    }
}
