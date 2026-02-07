using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SFSCtinstallerAPI.Models;
using SFSCtinstallerAPI.Utils;

namespace SFSCtinstallerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtifactController : ControllerBase {
    private readonly GithubApiService _githubApiService;

    // 构造函数注入 ApiHelper 服务
    public ArtifactController(GithubApiService githubApiService) {
        _githubApiService = githubApiService;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestArtifact() {
        try {
            var buildInfo = await _githubApiService.GetArtifacts();
            var buildList = buildInfo["artifacts"] as JArray;

            if (buildList == null || buildList.Count == 0)
                return NotFound(new ErrorMessage {
                    Message = "无法找到此构建"
                });

            var latestBuild = buildList[0];
            var artifactId = latestBuild["id"]?.ToString();

            if (string.IsNullOrEmpty(artifactId))
                return BadRequest(new ErrorMessage {
                    Message = "无效的构建 ID"
                });

            var stream = await _githubApiService.DownloadArtifact(artifactId);
            var fileName = $"{latestBuild["name"]}_{artifactId}.zip";
            return File(stream, "application/zip", fileName);
        } catch (Exception ex) {
            Console.WriteLine($"[Error] GetLatestArtifact: {ex}");
            return StatusCode(500, new ErrorMessage {
                Message = "构建产物获取失败"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArtifactById([FromRoute] string id) {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorMessage {
                Message = "构建 ID 不得为空"
            });

        try {
            var artifactInfo = await _githubApiService.GetArtifactById(id);
            if (artifactInfo == null)
                return NotFound($"Artifact {id} not found");

            var stream = await _githubApiService.DownloadArtifact(id);

            var fileName = $"{artifactInfo.Name}_{artifactInfo.Id}.zip";

            return File(stream, "application/zip", fileName);
        } catch (HttpRequestException httpEx) {
            Console.WriteLine($"[HTTP Error] GetArtifactById({id}): {httpEx}");
            return StatusCode(502, new ErrorMessage {
                Message = "无法下载构建产物"
            });
        } catch (Exception ex) {
            Console.WriteLine($"[Error] GetArtifactById({id}): {ex}");
            return StatusCode(500, new ErrorMessage {
                Message = ex.Message
            });
        }
    }
}