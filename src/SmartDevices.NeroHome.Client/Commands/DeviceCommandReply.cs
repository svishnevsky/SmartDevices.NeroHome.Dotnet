namespace SmartDevices.NeroHome.Client.Commands;

public class DeviceCommandReply
{
    public DeviceCommandReply(string label)
    {
        this.Label = label;
    }

    public string Label { get; }
}
