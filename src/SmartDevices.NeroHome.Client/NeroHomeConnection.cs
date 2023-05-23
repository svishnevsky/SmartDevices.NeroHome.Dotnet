using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDevices.NeroHome.Client;

public class NeroHomeConnection :
    IDisposable,
    INeroHomeConnection
{
    private static readonly RemoteCertificateValidationCallback
        trustCertificateCallback = new((_, _, _, _) => true);
    private readonly NeroHomeServerSettings serverSettings;
    private TcpClient? client;
    private SslStream? sslStream;
    private StreamWriter? writer;
    private CancellationTokenSource? readCancellationToken;
    private bool disposedValue;

    public NeroHomeConnection(NeroHomeServerSettings serverSettings)
    {
        this.serverSettings = serverSettings;
    }

    public event Action<string>? OnServerMessage;

    public bool IsConnected => this.client?.Connected == true &&
        this.sslStream?.IsAuthenticated == true &&
        this.sslStream.CanRead &&
        this.sslStream.CanWrite;

    public async Task SendMessage(string data)
    {
        this.EnsureConnected();
        await this.writer!.WriteAsync(data);
        await this.writer.FlushAsync();
    }

    private void EnsureConnected()
    {
        if (this.IsConnected)
        {
            return;
        }

        lock (this.serverSettings)
        {
            if (this.IsConnected)
            {
                return;
            }

            this.readCancellationToken?.Cancel();
            this.client = new(
                this.serverSettings.ServerAddress,
                this.serverSettings.Port);
            this.sslStream = !this.serverSettings.TrustServerCertificate
                ? new(
                    this.client.GetStream(),
                    false)
                : new(
                    this.client.GetStream(),
                    false,
                    trustCertificateCallback,
                    null);
            this.sslStream.AuthenticateAsClient(
                this.serverSettings.ServerAddress,
                null,
                SslProtocols.Tls12,
                true);
            this.writer = new(this.sslStream);
            this.readCancellationToken = new CancellationTokenSource();
            this.StartReading(this.sslStream, this.readCancellationToken.Token);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.readCancellationToken?.Dispose();
                this.client?.Dispose();
            }

            this.disposedValue = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void StartReading(
        SslStream sslStream,
        CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            using StreamReader reader = new(sslStream);
            while (sslStream.CanRead &&
                !cancellationToken.IsCancellationRequested)
            {
                StringBuilder replyBuilder = new();
                int openBracketsCount = 0;
                do
                {
                    char currentSymbol = (char)reader.Read();
                    if (currentSymbol == '{')
                    {
                        openBracketsCount++;
                    }
                    else if (currentSymbol == '}')
                    {
                        openBracketsCount--;
                    }

                    replyBuilder.Append(currentSymbol);
                }
                while (openBracketsCount > 0);

                OnServerMessage?.Invoke(replyBuilder.ToString());
            }
        },
        cancellationToken);
    }
}
