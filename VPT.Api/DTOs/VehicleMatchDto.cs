namespace VPT.Api.DTOs;

public class VehicleMatchDto
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public decimal MatchScore { get; set; }
    public DateTime MatchedAt { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Mileage { get; set; }
    public decimal Price { get; set; }
    public string? Url { get; set; }
}
