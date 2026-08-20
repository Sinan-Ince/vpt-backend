namespace VPT.Api.DTOs;

public class VehicleTrackingDto
{
    public int VehicleSearchId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastCheckedAt { get; set; }

    public DateTime? NextCheckAt { get; set; }
}
