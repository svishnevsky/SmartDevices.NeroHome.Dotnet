namespace SmartDevices.NeroHome.Client.Commands;

public class NetroDeviceListCommand : NetroCommand<NetroDeviceListReply>
{
    public NetroDeviceListCommand()
        : base("getModemParams")
    {
    }

    public bool SavedIds { get; } = true;
}
