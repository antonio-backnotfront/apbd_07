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
    public IActionResult GetAllDevices()
    {
        var devices = _deviceService.GetAllDevices();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public IActionResult GetDevice(int id)
    {
        var device = _deviceService.GetDeviceById(id);
        if (device == null)
            return NotFound();
        return Ok(device);
    }

    [HttpPost]
    public IActionResult AddDevice([FromBody] Device device)
    {
        var newId = _deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetDevice), new { id = newId }, device);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDevice(int id, [FromBody] Device device)
    {
        if (device.Id != id)
            return BadRequest();

        _deviceService.UpdateDevice(device);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDevice(int id)
    {
        _deviceService.DeleteDevice(id);
        return NoContent();
    }
}