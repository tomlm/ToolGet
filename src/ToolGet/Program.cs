using Avalonia;
using Consolonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolGet.Core.Services;
using ToolGet.Core.ViewModels;

namespace ToolGet.Console;

internal class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        BuildAvaloniaApp()
            .StartWithConsoleLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        var host = CreateHostBuilder([]).Build();
        
        return AppBuilder.Configure(() => new App(host.Services))
            .UseConsolonia()
            .UseAutoDetectedConsole()
            .LogToTrace();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient<INuGetService, NuGetService>();
                services.AddTransient<SearchViewModel>();
            });
}
