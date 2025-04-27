using System.Text.RegularExpressions;

namespace Models;

public class EmbeddedDevice : Device
{
    private string _ipAddress = string.Empty;
    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (!Regex.IsMatch(value, @"^\b(?:\d{1,3}\.){3}\d{1,3}\b$"))
                throw new ArgumentException("Invalid IP Address format.");
            _ipAddress = value;
        }
    }

    public required string NetworkName { get; set; }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd."))
            throw new Exception("ConnectionException: Network name must contain 'MD Ltd.'.");
    }

    public override void TurnOn()
    {
        Connect();
        IsTurnedOn = true;
    }

    public override string ToString() =>
        $"Embedded Device [ID: {Id}, Name: {Name}, IP: {IpAddress}, Network: {NetworkName}, On: {IsTurnedOn}]";
}