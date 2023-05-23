using System;
using System.Text.Json;

namespace SmartDevices.NeroHome.Client.Commands;

public sealed class ServerAnswer : ServerMessage
{
    public ServerAnswer(
        CommandClasses @class,
        string command,
        ServerAnswerSatuses status,
        string? label = null,
        string? error = null,
        string? errorDescription = null)
        : base(@class, command, label)
    {
        this.Status = status;
        this.Error = error;
        this.ErrorDescription = errorDescription;
    }

    public ServerAnswerSatuses Status { get; }

    public string? Error { get; }

    public string? ErrorDescription { get; }

    public string? RawMessage { get; private set; }

    public TReply As<TReply>()
        where TReply : DeviceCommandReply
    {
        return JsonSerializer.Deserialize<TReply>(
            this.RawMessage ??
                throw new NullReferenceException($"{nameof(this.RawMessage)} is null."),
            JsonOptions)!;
    }

    public static ServerAnswer Parse(string message)
    {
        ServerAnswer answer = JsonSerializer.Deserialize<ServerAnswer>(
            message,
            JsonOptions)!;
        answer.RawMessage = message;

        return answer;
    }
}
