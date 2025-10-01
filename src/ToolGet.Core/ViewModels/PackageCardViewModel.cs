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
    private string _id = string.Empty;

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _authors = string.Empty;

    [ObservableProperty]
    private long _totalDownloads;

    [ObservableProperty]
    private string _projectUrl = string.Empty;

    [ObservableProperty]
    private string _iconUrl = string.Empty;

    [ObservableProperty]
    private string[] _tags = Array.Empty<string>();

    [ObservableProperty]
    private bool _isPrerelease;

    [ObservableProperty]
    private bool _isInstalling;

    [ObservableProperty]
    private string _installButtonText = "Install";

    [ObservableProperty]
    private string? _installedVersion;
    
    [ObservableProperty]
    private bool _isInstalled;

    [ObservableProperty]
    private bool _isUpdateAvailable;

    public PackageCardViewModel(NuGetPackage package, INuGetService nugetService, InstalledTool installedTool)
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
        IsInstalled = installedTool != null;
        InstalledVersion = installedTool?.Version!;
        IsUpdateAvailable = IsInstalled && InstalledVersion != Version;
    }

    [RelayCommand]
    private async Task InstallAsync()
    {
        if (IsInstalling) return;

        IsInstalling = true;

        try
        {
            var success = await _nugetService.InstallPackageAsync(Id, Version);
            if (success)
            {
                InstalledVersion = Version;
                IsInstalled = true;
                IsUpdateAvailable = false;
            }
        }
        finally
        {
            IsInstalling = false;
        }
    }


    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (IsInstalling) return;

        IsInstalling = true;

        try
        {
            var success = await _nugetService.UpdatePackageAsync(Id, Version);
            if (success)
            {
                InstalledVersion = Version;
                IsInstalled = true;
                IsUpdateAvailable = false;
            }
        }
        finally
        {
            IsInstalling = false;
        }
    }


    [RelayCommand]
    private async Task UnInstallAsync()
    {
        if (IsInstalling) return;

        IsInstalling = true;

        try
        {
            var success = await _nugetService.UnInstallPackageAsync(Id, Version);
            if (success)
            {
                InstalledVersion = null;
                IsInstalled = false;
                IsUpdateAvailable = false;
            }
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