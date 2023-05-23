using SmartDevices.NeroHome.Client;
using SmartDevices.NeroHome.Client.Commands;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var client = new NeroHomeClient(
            new NeroHomeServerCredentials(args[1], args[2]),
            new NeroHomeServerSettings(args[0], true));
        NetroDeviceListReply? result = await client
            .Send<NetroDeviceListCommand, NetroDeviceListReply>(new NetroDeviceListCommand());
        Console.ReadKey();
    }
}
