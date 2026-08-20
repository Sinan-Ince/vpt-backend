using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VPT.Api.Data;
using VPT.Api.DTOs;
using VPT.Api.Models;
using VPT.Api.Services;

namespace VPT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMatchingService _matchingService;
    private readonly IListingSource _listingSource;

    public ListingController(
        AppDbContext context,
        IMatchingService matchingService,
        IListingSource listingSource)
    {
        _context = context;
        _matchingService = matchingService;
        _listingSource = listingSource;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Listing listing)
    {
        _context.Listings.Add(listing);
        await _context.SaveChangesAsync();

        var searches = await _context.VehicleSearches.ToListAsync();

        await CreateMatchesAsync(listing, searches);

        return Ok(MapToDto(listing));
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportFromSource()
    {
        var listings = await _listingSource.FetchListingsAsync();

        _context.Listings.AddRange(listings);
        await _context.SaveChangesAsync();

        var searches = await _context.VehicleSearches.ToListAsync();

        foreach (var listing in listings)
        {
            await CreateMatchesAsync(listing, searches);
        }

        return Ok(listings.Select(MapToDto).ToList());
    }

    private async Task CreateMatchesAsync(Listing listing, List<VehicleSearch> searches)
    {
        foreach (var search in searches)
        {
            if (!_matchingService.IsCandidate(search, listing))
            {
                continue;
            }

            var score = _matchingService.CalculateMatchScore(
                search,
                listing);

            var match = new VehicleMatch
            {
                VehicleSearchId = search.Id,
                ListingId = listing.Id,
                MatchScore = score,
                MatchedAt = DateTime.UtcNow
            };

            _context.VehicleMatches.Add(match);
        }

        await _context.SaveChangesAsync();
    }

    private static ListingDto MapToDto(Listing listing)
    {
        return new ListingDto
        {
            Id = listing.Id,
            Brand = listing.Brand,
            Model = listing.Model,
            Year = listing.Year,
            Mileage = listing.Mileage,
            Price = listing.Price,
            Url = listing.Url
        };
    }
}