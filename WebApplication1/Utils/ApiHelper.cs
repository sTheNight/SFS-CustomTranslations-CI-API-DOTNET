using Newtonsoft.Json.Linq;

namespace SFSCtinstallerAPI.Utils {
    internal static class ApiHelper {
        private static readonly HttpClient HttpClient;

        static ApiHelper() {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.GITHUB_TOKEN}");
            HttpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Github-REST-API-Test");
        }

        public static async Task<JObject> GetArtifacts(int perPage = 1, int page = 1) {
            var url = $"https://api.github.com/repos/{Global.GITHUB_REPO_OWNER}/{Global.GITHUB_REPO_NAME}/actions/artifacts";
            var queryParams = new List<string> {
                $"per_page={perPage}",
                $"page={page}"
            };
            url += "?" + string.Join("&", queryParams);

            using var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        public static async Task<JObject> GetArtifactById(string artifactId) {
            if (string.IsNullOrEmpty(artifactId))
                throw new ArgumentNullException(nameof(artifactId));

            var url = $"https://api.github.com/repos/{Global.GITHUB_REPO_OWNER}/{Global.GITHUB_REPO_NAME}/actions/artifacts/{artifactId}";
            using var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
        public static async Task<Stream> DownloadArtifact(string artifactId) {
            var artifactInfo = await GetArtifactById(artifactId);
            var url = artifactInfo["archive_download_url"]?.ToString();
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("Artifact archive_download_url is null");

            var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
