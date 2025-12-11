using Dream_PC_Parts_Picker_API.DTOs.Parts;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dream_PC_Parts_Picker_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartService _service;

    public PartsController(IPartService service)
    {
        _service = service;
    }

    // GET: api/Parts?categoryId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartDto>>> GetAll([FromQuery] int? categoryId)
    {
        var parts = await _service.GetAllAsync(categoryId);

        var dtos = parts.Select(p => new PartDto(
            p.Id,
            p.PartCategoryId,
            p.PartCategory.Name,
            p.Name,
            p.Manufacturer,
            p.ModelNumber,
            p.Price,
            p.PerformanceScore,
            p.TdpWatts
        )).ToList();

        return Ok(dtos);
    }

    // GET: api/Parts/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartDto>> GetById(int id)
    {
        var part = await _service.GetByIdAsync(id);
        if (part == null) return NotFound();

        var dto = new PartDto(
            part.Id,
            part.PartCategoryId,
            part.PartCategory.Name,
            part.Name,
            part.Manufacturer,
            part.ModelNumber,
            part.Price,
            part.PerformanceScore,
            part.TdpWatts
        );

        return Ok(dto);
    }

    // POST: api/Parts
    [HttpPost]
    public async Task<ActionResult<PartDto>> Create([FromBody] CreatePartRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var part = await _service.CreateAsync(
            request.PartCategoryId,
            request.Name,
            request.Manufacturer,
            request.ModelNumber,
            request.Price,
            request.PerformanceScore,
            request.TdpWatts
        );

        if (part == null)
        {
            return BadRequest($"PartCategory with id {request.PartCategoryId} does not exist.");
        }

        var dto = new PartDto(
            part.Id,
            part.PartCategoryId,
            part.PartCategory.Name,
            part.Name,
            part.Manufacturer,
            part.ModelNumber,
            part.Price,
            part.PerformanceScore,
            part.TdpWatts
        );

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    // PUT: api/Parts/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var success = await _service.UpdateAsync(
            id,
            request.PartCategoryId,
            request.Name,
            request.Manufacturer,
            request.ModelNumber,
            request.Price,
            request.PerformanceScore,
            request.TdpWatts
        );

        if (!success)
        {
            return BadRequest("Part not found or PartCategory does not exist.");
        }

        return NoContent();
    }

    // DELETE: api/Parts/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
