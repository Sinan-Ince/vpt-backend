namespace VPT.Api.Models;

public class VehicleMatch
{
    public int Id { get; set; }

    public int VehicleSearchId { get; set; }

    public int ListingId { get; set; }

    public decimal MatchScore { get; set; }

    public DateTime MatchedAt { get; set; }

    public VehicleSearch VehicleSearch { get; set; } = null!;

    public Listing Listing { get; set; } = null!;

}