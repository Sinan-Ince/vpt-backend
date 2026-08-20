namespace VPT.Api.DTOs;

public class VehicleSearchDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }   

    public int? MaxMileage { get; set; }

    public decimal? MaxPrice { get; set; }  

    public string? FuelType { get; set; }

}