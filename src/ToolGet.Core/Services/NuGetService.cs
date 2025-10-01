using System.Diagnostics;
using System.Text.Json;
using ToolGet.Core.Models;
using CShellNet;
using static CShellNet.Globals;

namespace ToolGet.Core.Services;

public interface INuGetService
{
    Task<NuGetSearchResponse> SearchPackagesAsync(string query, int skip = 0, int take = 20);
    Task<bool> InstallPackageAsync(string packageId, string version);
    Task<bool> UnInstallPackageAsync(string packageId, string version);

    // Returns metadata for a single package id (or null if not found)
    Task<NuGetPackage?> GetPackageMetadataAsync(string packageId);

    // Update a globally installed dotnet tool (returns true on success)
    Task<bool> UpdatePackageAsync(string packageId, string version);
}

public class NuGetService : INuGetService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public NuGetService(HttpClient httpClient)
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<NuGetSearchResponse> SearchPackagesAsync(string query, int skip = 0, int take = 20)
    {
        try
        {
            var searchUrl = $"https://azuresearch-usnc.nuget.org/query?q={Uri.EscapeDataString(query)}&packageType=DotnetTool&skip={skip}&take={take}&prerelease=true";
            
            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var searchResponse = JsonSerializer.Deserialize<NuGetSearchResponse>(content, _jsonOptions);

            return searchResponse ?? new NuGetSearchResponse();
        }
        catch (Exception ex)
        {
            // In a real app, you would log this error
            Debug.WriteLine($"Error searching packages: {ex.Message}");
            return new NuGetSearchResponse();
        }
    }

    public async Task<NuGetPackage?> GetPackageMetadataAsync(string packageId)
    {
        if (string.IsNullOrWhiteSpace(packageId))
            return null;

        try
        {
            // Use the same search endpoint but restrict query to the exact package id.
            // Request a single result (take=1) and include prerelease so tools with prerelease versions can be found.
            var searchUrl = $"https://azuresearch-usnc.nuget.org/query?q=packageid:{Uri.EscapeDataString(packageId)}&packageType=DotnetTool&prerelease=true&take=1";

            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var searchResponse = JsonSerializer.Deserialize<NuGetSearchResponse>(content, _jsonOptions);

            if (searchResponse?.Data != null && searchResponse.Data.Length > 0)
            {
                return NuGetPackage.FromPackageData(searchResponse.Data[0]);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching package metadata for '{packageId}': {ex.Message}");
            return null;
        }
    }

    public async Task<bool> InstallPackageAsync(string packageId, string version)
    {
        try
        {
            var result = await Cmd("dotnet tool install -g " + packageId + (string.IsNullOrEmpty(version) ? "" : " --version " + version)).Execute();
            return result.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error installing package: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdatePackageAsync(string packageId, string version)
    {
        try
        {
            // Use dotnet tool update -g <packageId> [--version <version>]
            var cmd = "dotnet tool update -g " + packageId + (string.IsNullOrEmpty(version) ? "" : " --version " + version);
            var result = await Cmd(cmd).Execute();
            return result.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating package '{packageId}': {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UnInstallPackageAsync(string packageId, string version)
    {
        try
        {
            // var result = await Cmd("dotnet tool uninstall -g " + packageId + (string.IsNullOrEmpty(version) ? "" : " --version " + version)).Execute();
            var result = await Cmd($"dotnet tool uninstall -g {packageId}").Execute();
            return result.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error uninstalling package: {ex.Message}");
            return false;
        }
    }

}