namespace SudokuSolver.Web.Models;

public class GridContext
{
    public required int[,] Cells { get; init; }
    public void SolveSudoku()
    {

        int[,,] c = new int[11, 11, 11];
        int CellValue = 0, RelativePosition = 0, PossibleValues = 0;
        bool Solved = false; ;

        /*Check c(i,j,digit) possible valuies
        bool 
        PossibleValues = 0;
        CellValue = 0;
        for (int k = 1; k <= Settings.Dimension; k++)
        {
            PossibleValues++;
            CellValue = k;
        }*/


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
        do
        {
            for (var i = 1; i <= Settings.Dimension; i++)
            {
                for (var j = 1; j <= Settings.Dimension; j++)
                {
                    PossibleValues = 0;
                    CellValue = 0;
                    for (int k = 1; k <= Settings.Dimension; k++)
                    {
                        PossibleValues++;
                        CellValue = k;
                    }
                    if (PossibleValues == 1)
                    {
                        c[i, j, 10] = CellValue;
                        for (int k = 1; k <= Settings.Dimension; k++)
                        {
                            c[i, j, k] = 0;
                        }
                        break;
                    }
                }
            }
            //update line, column, zone restriction
            for (int index = 1; index <= Settings.Dimension; index++)
            {
                for (int j = 1; j <= Settings.Dimension; j++)
                {
                    PossibleValues = 0;
                    CellValue = 0;
                    RelativePosition = 0;
                    for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
                    {
                        PossibleValues++;
                        CellValue = tested_digit;
                        RelativePosition = j;
                    }
                }
                if (PossibleValues == 1)
                {
                    c[index, RelativePosition, 10] = CellValue;
                    for (int k = 1; k <= Settings.Dimension; k++)
                    {
                        c[index, RelativePosition, k] = 0;
                    }
                    break;
                }
                for (int i = 1; i <= Settings.Dimension; i++)
                {
                    PossibleValues = 0;
                    CellValue = 0;
                    RelativePosition = 0;
                    for (int tested_digit = 1; tested_digit <= Settings.Dimension; tested_digit++)
                    {
                        PossibleValues++;
                        CellValue = tested_digit;
                        RelativePosition = i;
                    }
                }
                if (PossibleValues == 1)
                {
                    c[RelativePosition, index, 10] = CellValue;
                    for (int k = 1; k <= Settings.Dimension; k++)
                    {
                        c[RelativePosition, index, k] = 0;
                    }
                    break;
                }
            }
            //Ckeck zones

            //special tests if no solution found

        } while (!Solved);


    }

}