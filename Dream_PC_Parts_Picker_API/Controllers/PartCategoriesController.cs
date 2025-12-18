using Dream_PC_Parts_Picker_API.Auth;
using Dream_PC_Parts_Picker_API.DTOs.PartCategories;
using Dream_PC_Parts_Picker_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dream_PC_Parts_Picker_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PartCategoriesController : ControllerBase
{
    private readonly IPartCategoryService _service;

    public PartCategoriesController(IPartCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartCategoryDto>>> GetAll()
    {
        var categories = await _service.GetAllAsync();

        var dtos = categories
            .Select(c => new PartCategoryDto(c.Id, c.Name, c.Description))
            .ToList();

        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartCategoryDto>> GetById(int id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();

        var dto = new PartCategoryDto(category.Id, category.Name, category.Description);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PartCategoryDto>> Create([FromBody] CreatePartCategoryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var category = await _service.CreateAsync(request.Name, request.Description);

        var dto = new PartCategoryDto(category.Id, category.Name, category.Description);

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartCategoryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var success = await _service.UpdateAsync(id, request.Name, request.Description);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
