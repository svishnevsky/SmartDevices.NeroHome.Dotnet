namespace SmartDevices.NeroHome.Client.Commands;

public class NetroDevice
{
    public NetroDevice(
        int id,
        bool route,
        bool saved,
        string @class)
    {
        this.Id = id;
        this.Route = route;
        this.Saved = saved;
        this.Class = @class;
    }

    public int Id { get; }

    public bool Route { get; }

    public bool Saved { get; }

    public string Class { get; }
}
