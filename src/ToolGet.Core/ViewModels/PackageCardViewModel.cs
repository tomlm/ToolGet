using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolGet.Core.Models;
using ToolGet.Core.Services;

namespace ToolGet.Core.ViewModels;

public partial class PackageCardViewModel : ObservableObject
{
    private readonly INuGetService _nugetService;

    [ObservableProperty]
    private string id = string.Empty;

    [ObservableProperty]
    private string version = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string authors = string.Empty;

    [ObservableProperty]
    private long totalDownloads;

    [ObservableProperty]
    private string projectUrl = string.Empty;

    [ObservableProperty]
    private string iconUrl = string.Empty;

    [ObservableProperty]
    private string[] tags = Array.Empty<string>();

    [ObservableProperty]
    private bool isPrerelease;

    [ObservableProperty]
    private bool isInstalling;

    [ObservableProperty]
    private string installButtonText = "Install";

    public PackageCardViewModel(NuGetPackage package, INuGetService nugetService)
    {
        _nugetService = nugetService;
        Id = package.Id;
        Version = package.Version;
        Title = package.Title;
        Description = package.Description;
        Authors = package.Authors;
        TotalDownloads = package.TotalDownloads;
        ProjectUrl = package.ProjectUrl;
        IconUrl = package.IconUrl;
        Tags = package.Tags;
        IsPrerelease = package.IsPrerelease;
    }

    [RelayCommand]
    private async Task InstallAsync()
    {
        if (IsInstalling) return;

        IsInstalling = true;
        InstallButtonText = "Installing...";

        try
        {
            var success = await _nugetService.InstallPackageAsync(Id, Version);
            if (success)
            {
                InstallButtonText = "Installed";
            }
            else
            {
                InstallButtonText = "Install Failed";
                await Task.Delay(2000);
                InstallButtonText = "Install";
            }
        }
        catch
        {
            InstallButtonText = "Install Failed";
            await Task.Delay(2000);
            InstallButtonText = "Install";
        }
        finally
        {
            IsInstalling = false;
        }
    }

    public string TagsString => Tags.Length > 0 ? string.Join(", ", Tags.Take(5)) : string.Empty;
    public string DownloadsString => TotalDownloads >= 1000000 
        ? $"{TotalDownloads / 1000000.0:F1}M" 
        : TotalDownloads >= 1000 
        ? $"{TotalDownloads / 1000.0:F1}K" 
        : TotalDownloads.ToString();
}