using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VPT.Api.Data;
using VPT.Api.DTOs;

namespace VPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("search/{searchId}")]
    public async Task<ActionResult<List<NotificationDto>>> GetForSearch(int searchId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.VehicleTracking.VehicleSearchId == searchId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                ListingId = n.ListingId,
                Brand = n.Listing.Brand,
                Model = n.Listing.Model,
                Year = n.Listing.Year,
                Price = n.Listing.Price,
                Url = n.Listing.Url
            })
            .ToListAsync();

        return Ok(notifications);
    }
}
