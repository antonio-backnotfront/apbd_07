namespace apbd_assignment_05_remake;

abstract public class Device
{
    
    
    public int Id { get; set; }
    public required string  Name { get; set; }
    
    /// <summary>
    /// Receive the boolean value that specifies whether the device is on or off
    /// </summary>
    public bool IsTurnedOn { get; set; }
    /// <summary>
    /// Turn on the device
    /// </summary>
    public abstract void TurnOn();
    /// <summary>
    /// Turn off the device
    /// </summary>
    public void TurnOff() => IsTurnedOn = false;
    public abstract override string ToString();
}