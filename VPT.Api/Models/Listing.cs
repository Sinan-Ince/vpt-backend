namespace VPT.Api.Models;

public class Listing
{
    public int Id { get; set; }
    
    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Mileage { get; set; }

    public decimal Price { get; set; }

    public string? Url { get; set; }

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<VehicleMatch> VehicleMatches { get; set; } = new List<VehicleMatch>();
}

