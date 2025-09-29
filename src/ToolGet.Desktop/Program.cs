using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolGet.Core.Services;
using ToolGet.Core.ViewModels;

namespace ToolGet.Desktop;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        var host = CreateHostBuilder([]).Build();
        
        return AppBuilder.Configure(() => new App(host.Services))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient<INuGetService, NuGetService>();
                services.AddTransient<MainViewModel>();
            });
}
