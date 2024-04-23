using System.Diagnostics;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SudokuSolver.Web.Models;

namespace SudokuSolver.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        await builder.Build().RunAsync();
    }
}