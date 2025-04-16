namespace apbd_assignment_05_remake;

/// <summary>
/// <param name="title">Device manage fjaifnainfkafa</param>
/// </summary>
public class DeviceManager
{
    private const int MaxDevices = 15;
    private List<Device> devices = new();
    private string filePath;

    public DeviceManager(string filePath)
    {
        this.filePath = filePath;
        LoadDevices();
    }

    private void LoadDevices()
    {
        if (!File.Exists(filePath)) return;

        foreach (var line in File.ReadAllLines(filePath, System.Text.Encoding.UTF8))
        {
            if (devices.Count >= MaxDevices)
            {
                // Console.WriteLine("Exceeding max devices");
                return;
            }

            var device = DeviceFactory.CreateDevice(line, devices.Count + 1);

            if (device == null)
            {
                // Console.WriteLine("Skipped invalid or unknown device type line: " + line);
                continue;
            }

            devices.Add(device);
            // Console.WriteLine(device);
        }
    }

    public List<Device> getDevices()
    {
        return devices;
    }

    
    /// <summary>
    /// Add device to a device manager
    /// </summary>
    /// <param name="device"></param>
    /// <exception cref="Exception"></exception>
    public void AddDevice(Device device)
    {
        if (devices.Count >= MaxDevices) throw new Exception("Storage is full!");
        devices.Add(device);
    }
    /// <summary>
    /// Remove device from the DeviceManager
    /// </summary>
    /// <param name="id"></param>
    public void RemoveDevice(int id) => devices.RemoveAll(d => d.Id == id);
    /// <summary>
    /// Show all the devices of the DeviceManaegr
    /// </summary>
    public void ShowDevices() => devices.ForEach(Console.WriteLine);
    
    /// <summary>
    /// Save all the devices in the file
    /// </summary>
    public void SaveDevices()
    {
        var lines = new List<string>();

        foreach (var device in devices)
        {
            switch (device)
            {
                case Smartwatch sw:
                    lines.Add($"SW-{sw.Id},{sw.Name},{sw.IsTurnedOn.ToString().ToLower()},{sw.BatteryPercentage}%");
                    break;
                case PersonalComputer pc:
                    lines.Add($"P-{pc.Id},{pc.Name},{pc.IsTurnedOn.ToString().ToLower()},{pc.OperatingSystem}");
                    break;
                case EmbeddedDevice ed:
                    lines.Add($"ED-{ed.Id},{ed.Name},{ed.IpAddress},{ed.NetworkName}");
                    break;
            }
        }

        File.WriteAllLines(filePath, lines);
    }

    
    /// <summary>
    /// Turn on the device by the specified id
    /// </summary>
    /// <param name="id"></param>
    public void TurnOn(int id) => devices.First(i => i.Id == id).TurnOn();
    
    /// <summary>
    /// Turn off the device by the specified id
    /// </summary>
    /// <param name="id"></param>
    public void TurnOff(int id) => devices.First(i => i.Id == id).TurnOff();
    
    /// <summary>
    /// Edit the device data by specifying the device id as the first argument
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newId"></param>
    /// <param name="name"></param>
    /// <param name="isTurnedOn"></param>
    public void EditDeviceData(int id, int newId, string name, bool isTurnedOn){
        devices.First(i => i.Id == id).Id = newId;
        devices.First(i => i.Id == newId).Name = name;
        devices.First(i => i.Id == newId).IsTurnedOn = isTurnedOn;
    }
    
}