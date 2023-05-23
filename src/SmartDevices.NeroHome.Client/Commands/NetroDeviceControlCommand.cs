namespace SmartDevices.NeroHome.Client.Commands;

public class NetroDeviceControlCommand : NetroCommand<DeviceCommandReply>
{
    public NetroDeviceControlCommand(
        int id,
        NetroDeviceControlActions action)
        : base("controlDevice")
    {
        this.Id = id;
        this.Action = action;
    }

    public NetroDeviceControlActions Action { get; }

    public int Id { get; }
}
