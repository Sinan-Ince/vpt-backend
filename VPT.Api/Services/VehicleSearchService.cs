using Microsoft.EntityFrameworkCore;
using VPT.Api.Data;
using VPT.Api.DTOs;
using VPT.Api.Models;

namespace VPT.Api.Services;

public class VehicleSearchService : IVehicleSearchService
{
    private readonly AppDbContext _context;
    private readonly IMatchingService _matchingService;

    public VehicleSearchService(AppDbContext context, IMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    public async Task<VehicleSearchDto> CreateAsync(CreateVehicleSearchDto dto)
    {
        var vehicleSearch = new VehicleSearch
        {
            UserId = dto.UserId,
            Brand = dto.Brand,
            Model = dto.Model,
            MinYear = dto.MinYear,
            MaxYear = dto.MaxYear,
            MaxMileage = dto.MaxMileage,
            MaxPrice = dto.MaxPrice,
            FuelType = dto.FuelType
        };

        _context.VehicleSearches.Add(vehicleSearch);

        await _context.SaveChangesAsync();

        var listings = await _context.Listings.ToListAsync();

        var matchedAny = false;

        foreach (var listing in listings)
        {
            if (!_matchingService.IsCandidate(vehicleSearch, listing))
            {
                continue;
            }

            var score = _matchingService.CalculateMatchScore(vehicleSearch, listing);

            _context.VehicleMatches.Add(new VehicleMatch
            {
                VehicleSearchId = vehicleSearch.Id,
                ListingId = listing.Id,
                MatchScore = score,
                MatchedAt = DateTime.UtcNow
            });

            matchedAny = true;
        }

        if (matchedAny)
        {
            await _context.SaveChangesAsync();
        }

        return MapToDto(vehicleSearch);
    }

    public async Task<VehicleSearchDto?> GetByIdAsync(int id)
    {
        var vehicleSearch = await _context.VehicleSearches
            .AsNoTracking()
            .FirstOrDefaultAsync(vs => vs.Id == id);

        if (vehicleSearch == null)
        {
            return null;
        }

        return MapToDto(vehicleSearch);
    }

    public async Task<List<VehicleSearchDto>> GetByUserIdAsync(int userId)
    {
        var vehicleSearches = await _context.VehicleSearches
            .AsNoTracking()
            .Where(vs => vs.UserId == userId)
            .ToListAsync();

        return vehicleSearches
            .Select(MapToDto)
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, CreateVehicleSearchDto dto)
    {
        var vehicleSearch = await _context.VehicleSearches
            .FirstOrDefaultAsync(vs => vs.Id == id);

        if (vehicleSearch == null)
        {
            return false;
        }

        vehicleSearch.UserId = dto.UserId;
        vehicleSearch.Brand = dto.Brand;
        vehicleSearch.Model = dto.Model;
        vehicleSearch.MinYear = dto.MinYear;
        vehicleSearch.MaxYear = dto.MaxYear;
        vehicleSearch.MaxMileage = dto.MaxMileage;
        vehicleSearch.MaxPrice = dto.MaxPrice;
        vehicleSearch.FuelType = dto.FuelType;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vehicleSearch = await _context.VehicleSearches
            .FirstOrDefaultAsync(vs => vs.Id == id);

        if (vehicleSearch == null)
        {
            return false;
        }

        _context.VehicleSearches.Remove(vehicleSearch);

        await _context.SaveChangesAsync();

        return true;
    }

    private static VehicleSearchDto MapToDto(VehicleSearch vehicleSearch)
    {
        return new VehicleSearchDto
        {
            Id = vehicleSearch.Id,
            UserId = vehicleSearch.UserId,
            Brand = vehicleSearch.Brand,
            Model = vehicleSearch.Model,
            MinYear = vehicleSearch.MinYear,
            MaxYear = vehicleSearch.MaxYear,
            MaxMileage = vehicleSearch.MaxMileage,
            MaxPrice = vehicleSearch.MaxPrice,
            FuelType = vehicleSearch.FuelType
        };
    }
}