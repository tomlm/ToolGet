using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ToolGet.Core.Services;
using ToolGet.Core.ViewModels;
using ToolGet.Desktop.Views;

namespace ToolGet.Desktop;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public App() : this(CreateEmptyServiceProvider())
    {
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateEmptyServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddHttpClient<INuGetService, NuGetService>();
        services.AddTransient<MainViewModel>();
        return services.BuildServiceProvider();
    }
}