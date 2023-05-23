using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartDevices.NeroHome.Client.Commands;

public abstract class ServerMessage
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    static ServerMessage()
    {
        JsonOptions.Converters.Add(new JsonStringEnumConverter(
            JsonNamingPolicy.CamelCase));
    }

    protected ServerMessage(
        CommandClasses @class,
        string command,
        string? label = null)
    {
        this.Class = @class;
        this.Command = command;
        this.Label = label;
    }

    public CommandClasses Class { get; }

    public string Command { get; }

    public string? Label { get; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(
            this,
            this.GetType(),
            JsonOptions);
    }
}
