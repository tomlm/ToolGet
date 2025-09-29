using System.Text.Json.Serialization;

namespace ToolGet.Core.Models;

public class NuGetSearchResponse
{
    [JsonPropertyName("data")]
    public NuGetPackageData[] Data { get; set; } = Array.Empty<NuGetPackageData>();

    [JsonPropertyName("totalHits")]
    public int TotalHits { get; set; }
}

public class NuGetPackageData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("authors")]
    public string[] Authors { get; set; } = Array.Empty<string>();

    [JsonPropertyName("totalDownloads")]
    public long TotalDownloads { get; set; }

    [JsonPropertyName("projectUrl")]
    public string ProjectUrl { get; set; } = string.Empty;

    [JsonPropertyName("iconUrl")]
    public string IconUrl { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [JsonPropertyName("versions")]
    public NuGetPackageVersion[] Versions { get; set; } = Array.Empty<NuGetPackageVersion>();
}

public class NuGetPackageVersion
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("@id")]
    public string Id { get; set; } = string.Empty;
}