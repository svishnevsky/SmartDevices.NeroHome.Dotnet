using System.Text.Json.Serialization;

namespace SmartDevices.NeroHome.Client.Commands;

public class NetroDeviceListReply : DeviceCommandReply
{
    public NetroDeviceListReply(
        string label,
        NetroDevice[] devices)
        : base(label)
    {
        this.Devices = devices;
    }

    [JsonPropertyName("savedIds")]
    public NetroDevice[] Devices { get; }
}
