using System.Diagnostics;
using System.Text.Json;
using ToolGet.Core.Models;

namespace ToolGet.Core.Services;

public interface INuGetService
{
    Task<NuGetSearchResponse> SearchPackagesAsync(string query, int skip = 0, int take = 20);
    Task<bool> InstallPackageAsync(string packageId, string version);
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
            // This would typically call dotnet tool install or dotnet add package
            // For now, we'll simulate the installation
            await Task.Delay(1000); // Simulate installation time
            
            // In a real implementation, you would execute:
            // dotnet tool install -g {packageId} --version {version}
            // or
            // dotnet add package {packageId} --version {version}
            
            Debug.WriteLine($"Simulating installation of {packageId} version {version}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error installing package: {ex.Message}");
            return false;
        }
    }
}