using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private async Task ShowInstalledAsync()
    {
        SearchResults.Clear();

        var installedTools = await GetInstalledTools();
        if (installedTools.Count > 0)
        {
            foreach (var installedTool in installedTools)
            {
                var package = await _nugetService.GetPackageMetadataAsync(installedTool.Id);
                if (package != null)
                {
                    var viewModel = new PackageCardViewModel(package, _nugetService, installedTool);
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

        var installedTools = await GetInstalledTools();

        IsSearching = true;
        StatusMessage = "Searching packages...";
        SearchResults.Clear();


        try
        {
            var packages = await _nugetService.SearchPackagesAsync(SearchQuery.Trim(), 0, 50);

            if (packages != null)
            {
                foreach (var package in packages)
                {
                    var viewModel = new PackageCardViewModel(package, _nugetService, installedTools.FirstOrDefault(tool => String.Compare(tool.Id, package.Identity.Id, true) == 0));
                    SearchResults.Add(viewModel);
                }
                StatusMessage = $"Found {packages.Count()} packages";
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

    private async Task<List<PackageReference>> GetInstalledTools()
    {
        try
        {
            Echo = false;
            var result = await Cmd("dotnet tool list -g").AsString();
            var tools = new List<PackageReference>();
            var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(2)) // Skip header lines
            {
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    tools.Add(new PackageReference
                    {
                        Id = parts[0],
                        Version = parts.Length > 1 ? parts[1] : throw new ArgumentNullException("Missing version")
                    });
                }
            }
            return tools;
        }
        catch
        {
            return new List<PackageReference>();
        }
    }
}

public class PackageReference
{
    public string Id { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}