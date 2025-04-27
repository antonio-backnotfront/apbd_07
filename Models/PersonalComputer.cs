namespace Models;

public class PersonalComputer : Device
{
    public string? OperatingSystem { get; set; }

    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
            throw new Exception("EmptySystemException: No OS installed.");
        IsTurnedOn = true;
    }

    public override string ToString() =>
        $"PC [ID: {Id}, Name: {Name}, OS: {OperatingSystem ?? "None"}, On: {IsTurnedOn}]";
}