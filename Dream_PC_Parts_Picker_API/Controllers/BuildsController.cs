using System.Security.Claims;
using Dream_PC_Parts_Picker_API.Auth;
using Dream_PC_Parts_Picker_API.DTOs.Benchmarks;
using Dream_PC_Parts_Picker_API.DTOs.Builds;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dream_PC_Parts_Picker_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireApiKey]
public class BuildsController : ControllerBase
{
    private readonly IBuildService _buildService;

    public BuildsController(IBuildService buildService)
    {
        _buildService = buildService;
    }

    private int? GetUserId()
    {
        var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idStr, out var id)) return id;
        return null;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BuildSummaryDto>>> GetMyBuilds()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var builds = await _buildService.GetBuildsForUserAsync(userId.Value);

        var summaries = builds.Select(b =>
        {
            var totalPrice = b.BuildParts.Sum(bp => bp.Part.Price * bp.Quantity);
            var partsCount = b.BuildParts.Sum(bp => bp.Quantity);

            return new BuildSummaryDto(
                b.Id,
                b.Name,
                b.Description,
                totalPrice,
                partsCount,
                b.CreatedAt
            );
        }).ToList();

        return Ok(summaries);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BuildDetailDto>> GetBuild(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var build = await _buildService.GetBuildForUserAsync(userId.Value, id);
        if (build == null) return NotFound();

        var totalPrice = build.BuildParts.Sum(bp => bp.Part.Price * bp.Quantity);

        var parts = build.BuildParts.Select(bp =>
            new BuildPartItemDto(
                bp.PartId,
                bp.Part.Name,
                bp.Part.PartCategory.Name,
                bp.Part.Price,
                bp.Quantity
            )
        ).ToList();

        var benchmarks = build.Benchmarks
            .OrderByDescending(bb => bb.CreatedAt)
            .Select(bb => new BuildBenchmarkSummaryDto(
                bb.Id,
                bb.OverallScore,
                bb.CpuScore,
                bb.GpuScore,
                bb.CreatedAt
            ))
            .ToList();

        var dto = new BuildDetailDto(
            build.Id,
            build.Name,
            build.Description,
            totalPrice,
            build.CreatedAt,
            parts,
            benchmarks
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<BuildDetailDto>> CreateBuild([FromBody] CreateBuildRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var parts = request.Parts.Select(p => (p.PartId, p.Quantity)).ToList();

        var build = await _buildService.CreateBuildAsync(
            userId.Value,
            request.Name,
            request.Description,
            parts
        );

        if (build == null)
        {
            return BadRequest("One or more parts do not exist.");
        }

        var totalPrice = build.BuildParts.Sum(bp => bp.Part.Price * bp.Quantity);
        var partDtos = build.BuildParts.Select(bp =>
            new BuildPartItemDto(
                bp.PartId,
                bp.Part.Name,
                bp.Part.PartCategory.Name,
                bp.Part.Price,
                bp.Quantity
            )
        ).ToList();

        var dto = new BuildDetailDto(
            build.Id,
            build.Name,
            build.Description,
            totalPrice,
            build.CreatedAt,
            partDtos,
            Array.Empty<BuildBenchmarkSummaryDto>()
        );

        return CreatedAtAction(nameof(GetBuild), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBuild(int id, [FromBody] UpdateBuildRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var parts = request.Parts.Select(p => (p.PartId, p.Quantity)).ToList();

        var success = await _buildService.UpdateBuildAsync(
            userId.Value,
            id,
            request.Name,
            request.Description,
            parts
        );

        if (!success)
        {
            return BadRequest("Build not found or one or more parts do not exist.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBuild(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _buildService.DeleteBuildAsync(userId.Value, id);
        if (!success) return NotFound();

        return NoContent();
    }
}
