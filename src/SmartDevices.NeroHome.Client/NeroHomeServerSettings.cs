namespace SmartDevices.NeroHome.Client;

public class NeroHomeServerSettings
{
    public NeroHomeServerSettings(
        string serverAddress,
        bool trustServerCertificate = false,
        int port = 1333)
    {
        this.ServerAddress = serverAddress;
        this.Port = port;
        this.TrustServerCertificate = trustServerCertificate;
    }

    public string ServerAddress { get; }

    public int Port { get; }

    public bool TrustServerCertificate { get; }
}
