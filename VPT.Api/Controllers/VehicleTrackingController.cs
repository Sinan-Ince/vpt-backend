using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VPT.Api.Data;
using VPT.Api.DTOs;
using VPT.Api.Models;

namespace VPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleTrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehicleTrackingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<TrackedVehicleDto>>> GetActiveByUserId(int userId)
    {
        var tracked = await _context.VehicleTrackings
            .AsNoTracking()
            .Where(t => t.IsActive && t.VehicleSearch.UserId == userId)
            .Select(t => new TrackedVehicleDto
            {
                VehicleSearchId = t.VehicleSearchId,
                Brand = t.VehicleSearch.Brand,
                Model = t.VehicleSearch.Model,
                MinYear = t.VehicleSearch.MinYear,
                MaxYear = t.VehicleSearch.MaxYear,
                MaxMileage = t.VehicleSearch.MaxMileage,
                MaxPrice = t.VehicleSearch.MaxPrice,
                FuelType = t.VehicleSearch.FuelType,
                LastCheckedAt = t.LastCheckedAt,
                NextCheckAt = t.NextCheckAt,
                MatchCount = _context.VehicleMatches.Count(m => m.VehicleSearchId == t.VehicleSearchId)
            })
            .ToListAsync();

        return Ok(tracked);
    }

    [HttpGet("{searchId}")]
    public async Task<ActionResult<VehicleTrackingDto>> GetTracking(int searchId)
    {
        var tracking = await _context.VehicleTrackings
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.VehicleSearchId == searchId);

        if (tracking == null)
        {
            return Ok(new VehicleTrackingDto
            {
                VehicleSearchId = searchId,
                IsActive = false
            });
        }

        return Ok(MapToDto(tracking));
    }

    [HttpPut("{searchId}")]
    public async Task<ActionResult<VehicleTrackingDto>> SetTracking(
        int searchId,
        SetVehicleTrackingDto dto)
    {
        var search = await _context.VehicleSearches.FindAsync(searchId);

        if (search == null)
        {
            return NotFound("VehicleSearch bulunamadı.");
        }

        var tracking = await _context.VehicleTrackings
            .FirstOrDefaultAsync(t => t.VehicleSearchId == searchId);

        if (tracking == null)
        {
            tracking = new VehicleTracking
            {
                VehicleSearchId = searchId,
                CreatedAt = DateTime.UtcNow
            };

            _context.VehicleTrackings.Add(tracking);
        }

        tracking.IsActive = dto.IsActive;

        // Aktif edildiğinde bir sonraki tick'te hemen kontrol edilsin diye
        // NextCheckAt "şimdi" olarak ayarlanıyor; TrackingBackgroundService
        // her taramadan sonra bunu ileri bir tarihe güncelliyor.
        tracking.NextCheckAt = dto.IsActive ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();

        return Ok(MapToDto(tracking));
    }

    private static VehicleTrackingDto MapToDto(VehicleTracking tracking)
    {
        return new VehicleTrackingDto
        {
            VehicleSearchId = tracking.VehicleSearchId,
            IsActive = tracking.IsActive,
            LastCheckedAt = tracking.LastCheckedAt,
            NextCheckAt = tracking.NextCheckAt
        };
    }
}
