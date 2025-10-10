using CShellNet;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;
using ToolGet.Core.ViewModels;
using static CShellNet.Globals;

namespace ToolGet.Core.Services;

public interface INuGetService
{
    Task<IEnumerable<IPackageSearchMetadata>> SearchPackagesAsync(string query, int skip = 0, int take = 20);
    Task<bool> InstallPackageAsync(string packageId, string version);
    Task<bool> UnInstallPackageAsync(string packageId, string version);

    // Returns metadata for a single package id (or null if not found)
    Task<IPackageSearchMetadata> GetPackageMetadataAsync(string packageId);

    // Update a globally installed dotnet tool (returns true on success)
    Task<bool> UpdatePackageAsync(string packageId, string version);
}

public class NuGetService : INuGetService
{
    private SourceRepository _repo;

    public NuGetService()
    {
        var source = new PackageSource("https://api.nuget.org/v3/index.json");
        var providers = Repository.Provider.GetCoreV3();
        _repo = new SourceRepository(source, providers);

    }

    public async Task<IEnumerable<IPackageSearchMetadata>> SearchPackagesAsync(string query, int skip = 0, int take = 20)
    {
        var results = await Cmd($"dotnet tool search \\\"{query}\\\" --prerelease").AsString();
        var lines = results.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(2).ToList();
        if (lines.Count == 0)
            return Enumerable.Empty<IPackageSearchMetadata>();

        var searchResults = new List<IPackageSearchMetadata>();
        foreach(var line in lines)
        {
            var parts = line.Split(' ');
            var packageReference = new PackageReference() { Id = parts[0], Version = parts[1] };
            var packageSearchMetadata = await GetPackageMetadataAsync(packageReference.Id);
            if (packageSearchMetadata != null)
                searchResults.Add(packageSearchMetadata);
        }
        return searchResults;
    }

    public async Task<IPackageSearchMetadata?> GetPackageMetadataAsync(string packageId)
    {
        if (string.IsNullOrWhiteSpace(packageId))
            return null;

        try
        {
            var metadataResource = await _repo.GetResourceAsync<PackageMetadataResource>();

            var metadataList = await metadataResource.GetMetadataAsync(
                packageId,                  // Package ID
                includePrerelease: true,    // Filter prerelease if needed
                includeUnlisted: false,      // Only listed packages
                new SourceCacheContext(),
                NullLogger.Instance,
                CancellationToken.None
            );

            // Get the latest version
            var latest = metadataList
                .OrderByDescending(m => m.Identity.Version)
                .FirstOrDefault();


            return latest;
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