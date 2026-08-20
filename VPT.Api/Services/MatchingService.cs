using VPT.Api.Models;

namespace VPT.Api.Services;

public class MatchingService : IMatchingService
{
    public bool IsCandidate(
        VehicleSearch search,
        Listing listing)
    {
        // Brand ve Model kesinlikle uyuşmalı; uyuşmuyorsa listing aday bile sayılmaz.
        bool brandMatches = string.Equals(
            search.Brand,
            listing.Brand,
            StringComparison.OrdinalIgnoreCase);

        bool modelMatches = string.Equals(
            search.Model,
            listing.Model,
            StringComparison.OrdinalIgnoreCase);

        return brandMatches && modelMatches;
    }

    public decimal CalculateMatchScore(
        VehicleSearch search,
        Listing listing)
    {
        if (!IsCandidate(search, listing))
        {
            return 0m;
        }

        int matchedCriteria = 0;
        int totalCriteria = 3;

        // Year
        if (listing.Year == search.Year)
        {
            matchedCriteria++;
        }

        // Mileage
        if (!search.MaxMileage.HasValue ||
            listing.Mileage <= search.MaxMileage.Value)
        {
            matchedCriteria++;
        }

        // Price
        if (!search.MaxPrice.HasValue ||
            listing.Price <= search.MaxPrice.Value)
        {
            matchedCriteria++;
        }

        return Math.Round(
            (decimal)matchedCriteria / totalCriteria * 100,
            2);
    }
}