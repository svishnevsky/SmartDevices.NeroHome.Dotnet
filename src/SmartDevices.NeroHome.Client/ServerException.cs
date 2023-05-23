using System;

namespace SmartDevices.NeroHome.Client;

public class ServerException : Exception
{
    public ServerException(string message, string? description)
        : base(message)
    {
        this.Description = description;
    }

    public string? Description { get; set; }
}
