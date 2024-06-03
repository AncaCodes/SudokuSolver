namespace SudokuSolver.Web.Models;

public static class Settings
{
    public const int Dimension = 9;
    public const int MinCellValue = 1;
    public const int MaxCellValue = 10;
    public const int SRootDimension = 3;//square root of Settings.Dimmension
    public const int TotalCellsCount = 9 * 9;
}