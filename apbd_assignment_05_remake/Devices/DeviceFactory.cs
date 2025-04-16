namespace apbd_assignment_05_remake;

static class DeviceFactory
{
    public static Device? CreateDevice(string line, int id)
    {
        var parts = line.Split(',');
        if (parts.Length < 3) return null;

        string typePrefix = parts[0].Split('-')[0];

        try
        {
            return typePrefix switch
            {
                "SW" when parts.Length >= 4 => new Smartwatch
                {
                    Id = id,
                    Name = parts[1],
                    IsTurnedOn = parts[2].ToLower() == "true",
                    BatteryPercentage = int.Parse(parts[3].TrimEnd('%'))
                },
                "P" when parts.Length >= 4 => new PersonalComputer
                {
                    Id = id,
                    Name = parts[1],
                    IsTurnedOn = parts[2].ToLower() == "true",
                    OperatingSystem = parts[3]
                },
                "ED" when parts.Length >= 4 && parts[3].Contains("MD Ltd.") => new EmbeddedDevice
                {
                    Id = id,
                    Name = parts[1],
                    IpAddress = parts[2],
                    NetworkName = parts[3]
                },
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }
}