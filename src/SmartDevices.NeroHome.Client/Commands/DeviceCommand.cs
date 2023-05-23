using System;

namespace SmartDevices.NeroHome.Client.Commands;

public abstract class DeviceCommand<TReply> : ServerMessage
    where TReply : DeviceCommandReply
{
    protected DeviceCommand(
        CommandClasses @class,
        string command)
        : base(@class, command, Guid.NewGuid().ToString("N"))
    {
    }
}
