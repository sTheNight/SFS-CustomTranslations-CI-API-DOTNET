using Newtonsoft.Json;

namespace SFSCtinstallerAPI.Models;

public class GithubArtifact
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("node_id")]
    public string NodeId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("size_in_bytes")]
    public long SizeInBytes { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("archive_download_url")]
    public string ArchiveDownloadUrl { get; set; }

    [JsonProperty("expired")]
    public bool Expired { get; set; }

    [JsonProperty("digest")]
    public string Digest { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [JsonProperty("workflow_run")]
    public WorkflowRun WorkflowRun { get; set; }
}