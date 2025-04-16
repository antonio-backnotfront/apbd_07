namespace apbd_assignment_05_remake;

public class Smartwatch : Device, IPowerNotifier
{
    private int batteryPercentage;
    public int BatteryPercentage
    {
        get => batteryPercentage;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException("Battery percentage must be between 0 and 100.");
            batteryPercentage = value;
            if (batteryPercentage < 20) NotifyLowBattery();
        }
    }

    /// <summary>
    /// Print the information about the low battery
    /// </summary>
    public void NotifyLowBattery() => Console.WriteLine("Warning: Battery below 20%!");

    
    
    public override void TurnOn()
    {
        if (BatteryPercentage < 11)
            throw new Exception("EmptyBatteryException: Battery too low to turn on.");
        IsTurnedOn = true;
        BatteryPercentage -= 10;
    }
    public override string ToString() => $"Smartwatch [ID: {Id}, Name: {Name}, Battery: {BatteryPercentage}%, On: {IsTurnedOn}]";
}

interface IPowerNotifier
{
    void NotifyLowBattery();
}
