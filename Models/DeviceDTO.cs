namespace Models;

public class DeviceDto
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsTurnedOn { get; set; }

    // Optional fields depending on type
    public int? BatteryPercentage { get; set; } // For Smartwatch
    public string? OperatingSystem { get; set; } // For Personal Computer
    public string? IpAddress { get; set; } // For Embedded Device
    public string? NetworkName { get; set; } // For Embedded Device
}