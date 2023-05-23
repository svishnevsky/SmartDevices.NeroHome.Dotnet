using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using SmartDevices.NeroHome.Client.Commands;

namespace SmartDevices.NeroHome.Client;

public class NeroHomeClient :
    INeroHomeClient,
    IDisposable
{
    private readonly ConcurrentDictionary<
        string,
        TaskCompletionSource<ServerAnswer>> subscribers = new();
    private readonly TimeSpan responseTimeout;
    private readonly INeroHomeConnection connection;
    private readonly NeroHomeServerCredentials credentials;
    private bool disposedValue;

    public NeroHomeClient(
        NeroHomeServerCredentials credentials,
        NeroHomeServerSettings serverSettings)
        : this(credentials, new NeroHomeConnection(serverSettings))
    {
    }

    public NeroHomeClient(
        NeroHomeServerCredentials credentials,
        INeroHomeConnection connection,
        TimeSpan? responseTimeout = null)
    {
        this.credentials = credentials;
        this.connection = connection;
        this.responseTimeout = responseTimeout ?? TimeSpan.FromMinutes(1);
        connection.OnServerMessage += this.HandleServerMessage;
    }

    public Task<TReply> Send<TCommand, TReply>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : DeviceCommand<TReply>
        where TReply : DeviceCommandReply
    {
        return Policy
            .Handle<ServerException>()
            .RetryAsync(2, async (exception, attempt) =>
            {
                if (exception.Message == "command denied")
                {
                    await this.SendMessage<AuthCommand, DeviceCommandReply>(
                        new AuthCommand(
                            this.credentials.Login,
                            this.credentials.Password),
                        cancellationToken);
                }
            })
            .ExecuteAsync(() => this.SendMessage<TCommand, TReply>(
                command,
                cancellationToken));
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                if (this.connection is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            this.disposedValue = true;
        }
    }

    private async Task<TReply> SendMessage<TCommand, TReply>(
        TCommand command,
        CancellationToken cancellationToken)
        where TCommand : DeviceCommand<TReply>
        where TReply : DeviceCommandReply
    {
        var taskCompletionSource = new TaskCompletionSource<ServerAnswer>();
        this.subscribers.TryAdd(command.Label!, taskCompletionSource);
        await this.connection.SendMessage(command.ToString());
        await Task.WhenAny(
            taskCompletionSource.Task,
            Task.Delay(this.responseTimeout, cancellationToken));
        this.subscribers.TryRemove(
            command.Label!,
            out TaskCompletionSource<ServerAnswer> _);

        return taskCompletionSource.Task.Status switch
        {
            TaskStatus.RanToCompletion => taskCompletionSource
                .Task
                .Result
                .As<TReply>(),
            TaskStatus.Faulted => throw taskCompletionSource
                .Task
                .Exception
                .InnerException,
            _ => throw new OperationCanceledException("No response."),
        };
    }

    private void HandleServerMessage(string message)
    {
        var answer = ServerAnswer.Parse(message);
        if (string.IsNullOrEmpty(answer?.Label) ||
            !this.subscribers.TryGetValue(
                answer.Label,
                out TaskCompletionSource<ServerAnswer> subscriber))
        {
            return;
        }

        switch (answer.Status)
        {
            case ServerAnswerSatuses.Error:
                subscriber.TrySetException(
                    new ServerException(
                        answer.Error ?? "Unknown error",
                        answer.ErrorDescription));
                break;
            case ServerAnswerSatuses.Completed:
                subscriber.TrySetResult(answer);
                break;
            default:
                break;
        }
    }
}
