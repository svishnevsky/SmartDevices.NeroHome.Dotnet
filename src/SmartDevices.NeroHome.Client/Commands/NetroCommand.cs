namespace SmartDevices.NeroHome.Client.Commands;

public abstract class NetroCommand<TReply> : DeviceCommand<TReply>
    where TReply : DeviceCommandReply
{
    protected NetroCommand(string command)
        : base(CommandClasses.Netro, command)
    {
    }
}
