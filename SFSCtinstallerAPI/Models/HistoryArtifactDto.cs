using Newtonsoft.Json;

namespace SFSCtinstallerAPI.Models;

public class HistoryArtifactDto {
    [JsonProperty("total_count")]
    public int TotalCount { get; set; }
    [JsonProperty("artifacts")]
    public List<GithubArtifact> Artifacts { get; set; }
}