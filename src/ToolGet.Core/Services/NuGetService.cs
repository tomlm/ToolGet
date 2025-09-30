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