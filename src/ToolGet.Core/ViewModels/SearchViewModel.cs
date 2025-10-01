using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolGet.Core.Models;
using ToolGet.Core.Services;
using CShellNet;
using static CShellNet.Globals;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace ToolGet.Core.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly INuGetService _nugetService;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isSearching;

    [ObservableProperty]
    private bool hasSearched;

    [ObservableProperty]
    private string statusMessage = "Enter a search term and click Search to find NuGet packages";

    public ObservableCollection<PackageCardViewModel> SearchResults { get; } = new();

    public SearchViewModel(INuGetService nugetService)
    {
        _nugetService = nugetService;
    }

    [RelayCommand]
    private async Task ExitAsync()
    {
        var lifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        lifetime.Shutdown();
    }

    [RelayCommand]
    private async Task InstalledAsync()
    {
        SearchResults.Clear();

        var installedTools = await GetTools();
        if (installedTools.Count > 0)
        {
            foreach (var tool in installedTools)
            {
                var package = await _nugetService.GetPackageMetadataAsync(tool);
                if (package != null)
                {
                    var viewModel = new PackageCardViewModel(package, _nugetService, true);
                    SearchResults.Add(viewModel);
                }
            }
            StatusMessage = $"Found {SearchResults.Count} installed tools";
        }
        else
            StatusMessage = "No installed tools found";

    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (IsSearching || string.IsNullOrWhiteSpace(SearchQuery))
            return;

        var installedTools = await GetTools();

        IsSearching = true;
        StatusMessage = "Searching packages...";
        SearchResults.Clear();


        try
        {
            var response = await _nugetService.SearchPackagesAsync(SearchQuery.Trim(), 0, 50);

            if (response.Data.Length > 0)
            {
                foreach (var packageData in response.Data)
                {
                    var package = NuGetPackage.FromPackageData(packageData);
                    var viewModel = new PackageCardViewModel(package, _nugetService, installedTools.Contains(package.Id, StringComparer.OrdinalIgnoreCase));
                    SearchResults.Add(viewModel);
                }
                StatusMessage = $"Found {response.TotalHits} packages (showing {response.Data.Length})";
            }
            else
            {
                StatusMessage = "No packages found for your search term";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Search failed: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
            HasSearched = true;
        }
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchQuery = string.Empty;
        SearchResults.Clear();
        HasSearched = false;
        StatusMessage = "Enter a search term and click Search to find NuGet packages";
    }

    private async Task<List<string>> GetTools()
    {
        try
        {
            Echo = false;
            var result = await Cmd("dotnet tool list -g").AsString();
            var tools = new List<string>();
            var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(2)) // Skip header lines
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    tools.Add(parts[0]); // Tool id is the first part
                }
            }
            return tools;
        }
        catch
        {
            return new List<string>();
        }
    }
}