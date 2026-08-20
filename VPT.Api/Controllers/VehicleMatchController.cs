using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VPT.Api.Data;
using VPT.Api.Services;
using VPT.Api.DTOs;
using System.IO.Compression;

namespace VPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleMatchController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMatchingService _matchingService;

    public VehicleMatchController(
        AppDbContext context,
        IMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    [HttpPost("{searchId}/{listingId}")]
    public async Task<IActionResult> CreateMatch(
        int searchId,
        int listingId)
    {
        var search = await _context.VehicleSearches.FindAsync(searchId);

        if (search == null)
        {
            return NotFound("VehicleSearch bulunamadı.");
        }

        var listing = await _context.Listings.FindAsync(listingId);

        if (listing == null)
        {
            return NotFound("Listing bulunamadı.");
        }

        var existingMatch = await _context.VehicleMatches
            .FirstOrDefaultAsync(m =>
                m.VehicleSearchId == searchId &&
                m.ListingId == listingId
            );

        if (existingMatch != null)
        {
            return Ok(existingMatch);
        }

        var score = _matchingService.CalculateMatchScore(
            search,
            listing);

        var match = new VPT.Api.Models.VehicleMatch
        {
            VehicleSearchId = searchId,
            ListingId = listingId,
            MatchScore = score,
            MatchedAt = DateTime.UtcNow
        };

        _context.VehicleMatches.Add(match);

        await _context.SaveChangesAsync();

        return Ok(match);
    }

    [HttpGet("search/{searchId}")]
    public async Task<ActionResult<List<VehicleMatchDto>>> GetMatches(
        int searchId)
    {
        var search = await _context.VehicleSearches.FindAsync(searchId);

        if (search == null)
        {
            return NotFound("VehicleSearch bulunamadı.");
        }

        var existingMatches = await _context.VehicleMatches
            .Where(m => m.VehicleSearchId == searchId)
            .ToListAsync();

        _context.VehicleMatches.RemoveRange(existingMatches);

        var listings = await _context.Listings.ToListAsync();

        foreach (var listing in listings)
        {
            if (!_matchingService.IsCandidate(search, listing))
            {
                continue;
            }

            var score = _matchingService.CalculateMatchScore(search, listing);

            _context.VehicleMatches.Add(new VPT.Api.Models.VehicleMatch
            {
                VehicleSearchId = searchId,
                ListingId = listing.Id,
                MatchScore = score,
                MatchedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        var matches = await _context.VehicleMatches
            .Where(m => m.VehicleSearchId == searchId)
            .OrderByDescending(m => m.MatchScore)
            .Select(m => new VehicleMatchDto
            {
                Id = m.Id,
                ListingId = m.ListingId,
                MatchScore = m.MatchScore,
                MatchedAt = m.MatchedAt,
                Brand = m.Listing.Brand,
                Model = m.Listing.Model,
                Year = m.Listing.Year,
                Mileage = m.Listing.Mileage,
                Price = m.Listing.Price,
                Url = m.Listing.Url
            })
            .ToListAsync();

        return Ok(matches);
    }



}