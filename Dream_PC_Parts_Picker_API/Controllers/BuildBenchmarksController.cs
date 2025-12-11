using System.Security.Claims;
using Dream_PC_Parts_Picker_API.DTOs.Benchmarks;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dream_PC_Parts_Picker_API.Controllers;

[ApiController]
[Route("api/builds/{buildId:int}/[controller]")]
[Authorize]
public class BuildBenchmarksController : ControllerBase
{
    private readonly IBuildBenchmarkService _benchmarkService;

    public BuildBenchmarksController(IBuildBenchmarkService benchmarkService)
    {
        _benchmarkService = benchmarkService;
    }

    private int? GetUserId()
    {
        var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idStr, out var id)) return id;
        return null;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BuildBenchmarkDto>>> GetForBuild(int buildId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var benchmarks = await _benchmarkService.GetBenchmarksForBuildAsync(userId.Value, buildId);

        var dtos = benchmarks.Select(bb =>
            new BuildBenchmarkDto(
                bb.Id,
                bb.BuildId,
                bb.OverallScore,
                bb.CpuScore,
                bb.GpuScore,
                bb.Notes,
                bb.CreatedAt
            )
        ).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<BuildBenchmarkDto>> Create(int buildId, [FromBody] CreateBuildBenchmarkRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var benchmark = await _benchmarkService.CreateBenchmarkAsync(
            userId.Value,
            buildId,
            request.OverallScore,
            request.CpuScore,
            request.GpuScore,
            request.Notes
        );

        if (benchmark == null)
        {
            return BadRequest("Build not found or does not belong to the current user.");
        }

        var dto = new BuildBenchmarkDto(
            benchmark.Id,
            benchmark.BuildId,
            benchmark.OverallScore,
            benchmark.CpuScore,
            benchmark.GpuScore,
            benchmark.Notes,
            benchmark.CreatedAt
        );

        return CreatedAtAction(nameof(GetForBuild), new { buildId = benchmark.BuildId }, dto);
    }

    [HttpPut("{benchmarkId:int}")]
    public async Task<IActionResult> Update(
        int buildId,
        int benchmarkId,
        [FromBody] UpdateBuildBenchmarkRequest request
    )
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _benchmarkService.UpdateBenchmarkAsync(
            userId.Value,
            benchmarkId,
            request.OverallScore,
            request.CpuScore,
            request.GpuScore,
            request.Notes
        );

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{benchmarkId:int}")]
    public async Task<IActionResult> Delete(int buildId, int benchmarkId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _benchmarkService.DeleteBenchmarkAsync(userId.Value, benchmarkId);
        if (!success) return NotFound();

        return NoContent();
    }
}
