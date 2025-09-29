namespace ToolGet.Core.Models;

public class NuGetPackage
{
    public string Id { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public long TotalDownloads { get; set; }
    public string ProjectUrl { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public bool IsPrerelease { get; set; }

    public static NuGetPackage FromPackageData(NuGetPackageData data)
    {
        return new NuGetPackage
        {
            Id = data.Id,
            Version = data.Version,
            Title = string.IsNullOrWhiteSpace(data.Title) ? data.Id : data.Title,
            Description = data.Description,
            Authors = string.Join(", ", data.Authors),
            TotalDownloads = data.TotalDownloads,
            ProjectUrl = data.ProjectUrl,
            IconUrl = data.IconUrl,
            Tags = data.Tags,
            IsPrerelease = data.Version.Contains("-") || data.Version.Contains("alpha") || data.Version.Contains("beta")
        };
    }
}
