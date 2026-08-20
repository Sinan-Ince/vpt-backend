using System.ComponentModel.DataAnnotations;

namespace VPT.Api.DTOs;

public class CreateVehicleSearchDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    public int? MaxMileage { get; set; }

    public decimal? MaxPrice { get; set; }

    [MaxLength(30)]
    public string? FuelType { get; set; }
}