using Newtonsoft.Json;

namespace SFSCtinstallerAPI.Models;

public class WorkflowRun
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("repository_id")]
    public long RepositoryId { get; set; }

    [JsonProperty("head_repository_id")]
    public long HeadRepositoryId { get; set; }

    [JsonProperty("head_branch")]
    public string HeadBranch { get; set; }

    [JsonProperty("head_sha")]
    public string HeadSha { get; set; }
}