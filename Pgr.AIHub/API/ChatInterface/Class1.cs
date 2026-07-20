using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pgr.AIHub.API.ChatInterface;

public enum AiChatRole
{
    System,
    User,
    Assistant,
    Tool
}

public enum ContentType
{
    Selection
}

public interface IAiChatMessage
{
    AiChatRole Role { get; }

    string Content { get; }

}

public interface IAiChatContextItem
{
    string Title { get; }

    string? Content { get; }

    string? ContentType { get; }
}

public interface IAiChatOptions
{
    string Model { get; }

    string Mode { get; }

    string? ApiKey { get; }
}

public interface IAiChatRequest
{
    string Prompt { get; }
    IReadOnlyList<IAiChatContextItem> ContextItems { get; }

    IAiChatOptions Options { get; }

}

public sealed class AiChatMessageReceivedEventArgsDummyImp : EventArgs
{
    public AiChatMessageReceivedEventArgsDummyImp(IAiChatClientWorker sender, IAiChatMessage data)
    {
        Sender = sender;
        Message = data;
    }

    public IAiChatClientWorker Sender { get; }
    public IAiChatMessage Message { get; }
}

public interface IAiChatClientWorker
{
    event EventHandler<AiChatMessageReceivedEventArgsDummyImp>? MessageReceived;

    IReadOnlyList<IAiChatMessage> Messages { get; }

    public Guid Guid { get; }

    Task SendAsync(IAiChatRequest request, CancellationToken cancellationToken = default);
    Task SendAsync(string prompt, IAiChatOptions Options, CancellationToken cancellationToken = default);

    Task CancelAsync();
}

public interface IAiChatManager
{
    public IAiChatClientWorker CreateChat();
    public IReadOnlyList<IAiChatClientWorker> Chats { get; set; }
}


public interface IChatRateLimiter
{
    public Task DelayIfNeeded();
    public bool RequestStarted(string model);
    public bool RequestFinished(string model, double tokens);
}
interface IModelSepecificImplementation
{
    bool IsModelSupported(string model);
}

