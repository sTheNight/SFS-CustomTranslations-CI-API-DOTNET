using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SFSCtinstallerAPI.Utils;

namespace SFSCtinstallerAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase {
        private ApiHelper ApiHelper;
        // 构造函数注入 ApiHelper 服务
        public HistoryController(ApiHelper apiHelper) {
            ApiHelper = apiHelper;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? page = 1) {
            int pageNumber = page.GetValueOrDefault(1);
            if (pageNumber <= 0)
                return BadRequest("Page number must be a positive integer");

            try {
                var buildInfo = await ApiHelper.GetArtifacts(10, pageNumber);
                var buildList = buildInfo["artifacts"] as JArray;

                if (buildList == null || buildList.Count == 0)
                    return NotFound("No artifacts found for this page");

                return Ok(buildList);
            } catch (HttpRequestException httpEx) {
                Console.WriteLine($"[HTTP Error] HistoryController.Get(page={pageNumber}): {httpEx}");
                return StatusCode(502, "Failed to fetch artifacts from GitHub");
            } catch (Exception ex) {
                Console.WriteLine($"[Error] HistoryController.Get(page={pageNumber}): {ex}");
                return StatusCode(500, "Server error while fetching artifact history");
            }
        }
    }
}
