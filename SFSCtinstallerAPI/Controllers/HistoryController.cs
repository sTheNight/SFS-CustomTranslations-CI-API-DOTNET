using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SFSCtinstallerAPI.Models;
using SFSCtinstallerAPI.Utils;

namespace SFSCtinstallerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HistoryController : ControllerBase {
    private readonly GithubApiService _githubApiService;

    // 构造函数注入 ApiHelper 服务
    public HistoryController(GithubApiService githubApiService) {
        _githubApiService = githubApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? page = 1) {
        int pageNumber = page.GetValueOrDefault(1);
        if (pageNumber <= 0)
            return BadRequest(new ErrorMessage {
                Message = "无效页码"
            });

        try {
            var buildInfo = await _githubApiService.GetArtifacts(10, pageNumber);
            var buildList = buildInfo["artifacts"] as JArray;
            return Ok(buildList);
        } catch (HttpRequestException httpEx) {
            Console.WriteLine($"[HTTP Error] HistoryController.Get(page={pageNumber}): {httpEx}");
            return StatusCode(502, new ErrorMessage {
                Message = "无法获取到产物列表"
            });
        } catch (Exception ex) {
            Console.WriteLine($"[Error] HistoryController.Get(page={pageNumber}): {ex}");
            return StatusCode(500, new ErrorMessage {
                Message = ex.Message
            });
        }
    }
}