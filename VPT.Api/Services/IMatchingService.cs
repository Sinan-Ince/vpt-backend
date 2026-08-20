using VPT.Api.Models;

namespace VPT.Api.Services;

public interface IMatchingService
{
    bool IsCandidate(VehicleSearch search, Listing listing);

    decimal CalculateMatchScore(VehicleSearch search, Listing listing);
}
