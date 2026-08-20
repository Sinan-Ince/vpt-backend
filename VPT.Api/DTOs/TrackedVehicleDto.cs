namespace VPT.Api.DTOs;

public class TrackedVehicleDto
{
    public int VehicleSearchId { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public int? MaxMileage { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? FuelType { get; set; }

    public DateTime? LastCheckedAt { get; set; }

    public DateTime? NextCheckAt { get; set; }

    public int MatchCount { get; set; }
}
