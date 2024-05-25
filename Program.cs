using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SudokuSolver.Web.Models;
using SudokuSolver.Web.Services;


namespace SudokuSolver.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddTransient<ISudokuGridFactory, SudokuGridFactory>();
        builder.RootComponents.Add<App>("#app");

        await builder.Build().RunAsync();

        }

}