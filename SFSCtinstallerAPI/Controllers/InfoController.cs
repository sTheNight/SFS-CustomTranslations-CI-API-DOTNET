using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SFSCtinstallerAPI.Models;
using SFSCtinstallerAPI.Utils;

namespace SFSCtinstallerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfoController : ControllerBase {
    private readonly GithubApiService _githubApiService;

    // 构造函数注入 ApiHelper 服务
    public InfoController(GithubApiService githubApiService) {
        _githubApiService = githubApiService;
    }

    [HttpGet("latest")]
    // 获取最新构建信息
    public async Task<IActionResult> GetLatestInfo() {
        try {
            var buildInfo = await _githubApiService.GetArtifacts();
            var buildList = buildInfo["artifacts"] as JArray;

            if (buildList == null || buildList.Count == 0)
                return NotFound(new ErrorMessage {
                    Message = "无法找到构建产物"
                });

            return Ok(buildList[0]);
        } catch (HttpRequestException httpEx) {
            Console.WriteLine($"[HTTP Error] InfoController.GetLatestInfo: {httpEx}");
            return StatusCode(502, new ErrorMessage {
                Message = $"获取构建产物失败：{httpEx.Message}"
            });
        } catch (Exception ex) {
            Console.WriteLine($"[Error] InfoController.GetLatestInfo: {ex}");
            return StatusCode(500, new ErrorMessage {
                Message = ex.Message
            });
        }
    }

    // 根据 ID 获取指定构建信息
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInfoById([FromRoute] string id) {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorMessage {
                Message = "构建 ID 不得为空"
            });

        try {
            var buildInfo = await _githubApiService.GetArtifactById(id);
            if (buildInfo == null)
                return NotFound(new ErrorMessage {
                    Message = $"无法找到 ID 为 {buildInfo.Id} 的构建产物"
                });

            return Ok(buildInfo);
        } catch (HttpRequestException httpEx) {
            Console.WriteLine($"[HTTP Error] InfoController.GetInfoById({id}): {httpEx}");
            return StatusCode(502, new ErrorMessage {
                Message = $"获取构建产物失败：{httpEx.Message}"
            });
        } catch (Exception ex) {
            Console.WriteLine($"[Error] InfoController.GetInfoById({id}): {ex}");
            return StatusCode(500, new ErrorMessage {
                Message = ex.Message
            });
        }
    }
}