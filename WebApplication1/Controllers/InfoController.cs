using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SFSCtinstallerAPI.Utils;

namespace SFSCtinstallerAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase {
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestInfo() {
            try {
                var buildInfo = await ApiHelper.GetArtifacts();
                var buildList = buildInfo["artifacts"] as JArray;

                if (buildList == null || buildList.Count == 0)
                    return NotFound("No artifacts found");

                return Ok(buildList[0]);
            } catch (HttpRequestException httpEx) {
                Console.WriteLine($"[HTTP Error] InfoController.GetLatestInfo: {httpEx}");
                return StatusCode(502, "Failed to fetch artifacts from GitHub");
            } catch (Exception ex) {
                Console.WriteLine($"[Error] InfoController.GetLatestInfo: {ex}");
                return StatusCode(500, "Server error while fetching latest artifact info");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfoById([FromRoute] string id) {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Artifact ID cannot be empty");

            try {
                var buildInfo = await ApiHelper.GetArtifactById(id);
                if (buildInfo == null)
                    return NotFound($"Artifact {id} not found");

                return Ok(buildInfo);
            } catch (HttpRequestException httpEx) {
                Console.WriteLine($"[HTTP Error] InfoController.GetInfoById({id}): {httpEx}");
                return StatusCode(502, "Failed to fetch artifact info from GitHub");
            } catch (Exception ex) {
                Console.WriteLine($"[Error] InfoController.GetInfoById({id}): {ex}");
                return StatusCode(500, "Server error while fetching artifact info");
            }
        }
    }
}
