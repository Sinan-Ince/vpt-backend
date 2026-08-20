using Microsoft.AspNetCore.Mvc;
using VPT.Api.DTOs;
using VPT.Api.Services;

namespace VPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleSearchController : ControllerBase
{
    private readonly IVehicleSearchService _service;

    public VehicleSearchController(IVehicleSearchService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleSearchDto>> Create(
        CreateVehicleSearchDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleSearchDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<VehicleSearchDto>>> GetByUserId(
        int userId)
    {
        var result = await _service.GetByUserIdAsync(userId);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        CreateVehicleSearchDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}