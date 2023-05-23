using System;
using System.Threading.Tasks;

namespace SmartDevices.NeroHome.Client;

public interface INeroHomeConnection
{
    bool IsConnected { get; }

    event Action<string>? OnServerMessage;

    Task SendMessage(string data);
}
