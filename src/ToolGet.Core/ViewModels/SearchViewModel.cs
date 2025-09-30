using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolGet.Core.Models;
using ToolGet.Core.Services;

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
    private async Task SearchAsync()
    {
        if (IsSearching || string.IsNullOrWhiteSpace(SearchQuery)) 
            return;

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
                    var viewModel = new PackageCardViewModel(package, _nugetService);
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
}