namespace SudokuSolver.Web.Models
{
    public static class CheckFunctions
    {


        public static void FillDiagonal(int[,] cells)
        {

            for (int i = 0; i < Settings.Dimension; i = i + Settings.SRN)

                FillBox(i, i, cells);
        }

        public static bool UnUsedInBox(int rowStart, int colStart, int num, int[,] cells)
        {
            for (int i = 0; i < Settings.SRN; i++)
                for (int j = 0; j < Settings.SRN; j++)
                    if (cells[rowStart + i, colStart + j] == num)
                        return false;

            return true;
        }

        public static void FillBox(int row, int col, int[,] cells)
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
                    while (!UnUsedInBox(row, col, num, cells));

                    cells[row + i, col + j] = num;
                }
            }
        }


        public static bool CheckIfSafe(int i, int j, int num, int[,] cells)
        {
            return (UnUsedInRow(i, num, cells) &&
                    UnUsedInCol(j, num, cells) &&
                    UnUsedInBox(i - i % Settings.SRN, j - j % Settings.SRN, num, cells));
        }

        public static bool UnUsedInRow(int i, int num, int[,] cells)
        {
            for (int j = 0; j < Settings.Dimension; j++)
                if (cells[i, j] == num)
                    return false;
            return true;
        }

        public static bool UnUsedInCol(int j, int num, int[,] cells)
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

    }

}
