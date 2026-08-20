namespace VPT.Api.Models;


public class VehicleTracking
{
    public int Id { get; set; }

    public int VehicleSearchId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastCheckedAt { get; set; }

    public DateTime? NextCheckAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public VehicleSearch VehicleSearch { get; set; } = null!;

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

}