namespace VPT.Api.DTOs;

public class ListingDto
{
    public int Id { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Mileage { get; set; }

    public decimal Price { get; set; }

    public string? Url { get; set; }
}
