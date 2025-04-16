namespace apbd_assignment_05_remake;

public class DeviceDto
{
    public string Type { get; set; } // "SW", "P", or "ED"
    public string Name { get; set; }
    public bool IsTurnedOn { get; set; }

    // Optional fields depending on type
    public int? BatteryPercentage { get; set; } // Smartwatch
    public string? OperatingSystem { get; set; } // PC
    public string? IpAddress { get; set; } // Embedded
    public string? NetworkName { get; set; } // Embedded
}
