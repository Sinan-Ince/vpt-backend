namespace VPT.Api.Models;

public class VehicleSearch
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Brand { get; set; } = string.Empty; 

    public string Model { get; set; } = string.Empty;

    public int MinYear { get; set; }

    public int MaxYear { get; set; }

    public int? MaxMileage { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? FuelType { get; set; }

    public User User { get; set; } = null!;

    public VehicleTracking? VehicleTracking { get; set; }
}