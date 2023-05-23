using System.Threading;
using System.Threading.Tasks;
using SmartDevices.NeroHome.Client.Commands;

namespace SmartDevices.NeroHome.Client;

public interface INeroHomeClient
{
    Task<TReply> Send<TCommand, TReply>(
        TCommand command,
        CancellationToken cancellationToken = default)
            where TCommand : DeviceCommand<TReply>
            where TReply : DeviceCommandReply;
}
