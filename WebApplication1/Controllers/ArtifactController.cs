using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SFSCtinstallerAPI.Utils;

namespace SFSCtinstallerAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class ArtifactController : ControllerBase {
        private ApiHelper ApiHelper;
        // 构造函数注入 ApiHelper 服务
        public ArtifactController(ApiHelper _apiHelper) {
            ApiHelper = _apiHelper;
        }
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestArtifact() {
            try {
                var buildInfo = await ApiHelper.GetArtifacts();
                var buildList = buildInfo["artifacts"] as JArray;

                if (buildList == null || buildList.Count == 0)
                    return NotFound("No artifacts found");

                var latestBuild = buildList[0];
                var artifactId = latestBuild["id"]?.ToString();

                if (string.IsNullOrEmpty(artifactId))
                    return BadRequest("Invalid artifact ID");

                var stream = await ApiHelper.DownloadArtifact(artifactId);
                if (stream == null)
                    return NotFound("Artifact stream not found");

                var fileName = $"{latestBuild["name"]}_{artifactId}.zip";

                return File(stream, "application/zip", fileName);
            } catch (Exception ex) {
                Console.WriteLine($"[Error] GetLatestArtifact: {ex}");
                return StatusCode(500, "Failed to download latest artifact");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtifactById([FromRoute] string id) {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Artifact ID cannot be empty");

            try {
                var artifactInfo = await ApiHelper.GetArtifactById(id);
                if (artifactInfo == null)
                    return NotFound($"Artifact {id} not found");

                var stream = await ApiHelper.DownloadArtifact(id);
                if (stream == null)
                    return NotFound($"Artifact stream {id} not found");

                var fileName = $"{artifactInfo["name"]}_{artifactInfo["id"]}.zip";

                return File(stream, "application/zip", fileName);
            } catch (HttpRequestException httpEx) {
                Console.WriteLine($"[HTTP Error] GetArtifactById({id}): {httpEx}");
                return StatusCode(502, "Failed to fetch artifact from GitHub");
            } catch (Exception ex) {
                Console.WriteLine($"[Error] GetArtifactById({id}): {ex}");
                return StatusCode(500, "Server error while downloading artifact");
            }
        }
    }
}
