using Logic;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers;

[ApiController]
[Route("api/devices")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceRepository _deviceService;

    public DeviceController(IDeviceRepository deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDevices()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDevice(int id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device == null)
            return NotFound();
        return Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> AddDevice([FromBody] Device device)
    {
        var newDevice = await _deviceService.AddDeviceAsync(device);
        return CreatedAtAction(nameof(GetDevice), new { id = newDevice.Id }, newDevice);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDevice(int id, [FromBody] Device device)
    {
        var updated = await _deviceService.UpdateDeviceAsync(id, device);
        if (!updated)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        var deleted = await _deviceService.DeleteDeviceAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}