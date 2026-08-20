namespace VPT.Api.Models;

public class Notification
{
    public int Id { get; set; }

    public int VehicleTrackingId { get; set; }

    public int ListingId { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public VehicleTracking VehicleTracking { get; set; } = null!;

    public Listing Listing { get; set; } = null!;

}