using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFSCtinstallerAPI.Utils {
    internal static class ApiHelper {
        private static HttpClient HttpClientBuilder() {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.GITHUB_TOKEN}");
            httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Github-REST-API-Test");
            return httpClient;
        }
        public static async Task<JObject> GetArtifacts(int? perPage = 1, int? page = 1) {
            var httpClient = HttpClientBuilder();

            var url = $"https://api.github.com/repos/{Global.GITHUB_REPO_OWNER}/{Global.GITHUB_REPO_NAME}/actions/artifacts";

            if (perPage.HasValue || page.HasValue) {
                var queryParams = new List<string>();
                if (perPage.HasValue) queryParams.Add($"per_page={perPage.Value}");
                if (page.HasValue) queryParams.Add($"page={page.Value}");
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);
            }

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
        public static async Task<JObject> GetArtifactById(string artifactId) {
            var httpClient = HttpClientBuilder();
            if (string.IsNullOrEmpty(artifactId)) {
                throw new ArgumentNullException("Paramater artifactId could not be null");
            }
            var url = $"https://api.github.com/repos/{Global.GITHUB_REPO_OWNER}/{Global.GITHUB_REPO_NAME}/actions/artifacts/{artifactId}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        public static async Task<Stream> DownloadArtifact(string artifactId) {
            var httpClient = HttpClientBuilder();
            var artifactInfo = await GetArtifactById(artifactId);
            var url = artifactInfo["archive_download_url"];
            // 立即返回 Stream 流而非缓存
            var response = await httpClient.GetAsync(url.ToString(),HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}