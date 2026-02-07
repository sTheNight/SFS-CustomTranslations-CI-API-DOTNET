using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFSCtinstallerAPI.Configuration;
using SFSCtinstallerAPI.Models;

namespace SFSCtinstallerAPI.Utils;

public class GithubApiService {
    private readonly HttpClient _httpClient;
    private readonly GitHubSettings _settings;

    public GithubApiService(HttpClient httpClient, IOptions<GitHubSettings> options) {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    // 获取构建产物列表
    public async Task<JObject> GetArtifacts(int perPage = 1, int page = 1) {
        // 限制每页最大10个，并防止非法参数
        if (perPage > 10 || perPage <= 0)
            perPage = 1;
        if (page <= 0)
            page = 1;

        var url = $"repos/{_settings.GITHUB_REPO_OWNER}/{_settings.GITHUB_REPO_NAME}/actions/artifacts";
        var queryParams = new List<string> {
            $"per_page={perPage}",
            $"page={page}"
        };
        url += "?" + string.Join("&", queryParams);

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JObject.Parse(content);
    }

    // 根据 ID 获取单个构建产物信息
    public async Task<GithubArtifact?> GetArtifactById(string artifactId) {
        if (string.IsNullOrEmpty(artifactId))
            throw new ArgumentNullException("artifactId is null");

        var url = $"repos/{_settings.GITHUB_REPO_OWNER}/{_settings.GITHUB_REPO_NAME}/actions/artifacts/{artifactId}";
        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GithubArtifact>(content);
    }

    // 根据 ID 下载构建产物
    public async Task<Stream> DownloadArtifact(string artifactId) {
        var artifactInfo = await GetArtifactById(artifactId);
        if (artifactInfo == null) 
            throw new ArgumentException($"Artifact {artifactId} not found");
        
        var url = artifactInfo.ArchiveDownloadUrl;

        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Artifact archive_download_url is null");
        // 直接传输响应流，否则会拖慢请求速度并造成内存浪费
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }
}