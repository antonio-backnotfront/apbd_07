using Models;

namespace Logic;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAllDevices();
    Device? GetDeviceById(int id);
    int AddDevice(Device device);
    void UpdateDevice(Device device);
    void DeleteDevice(int id);
}