using System;
using System.Collections.Generic;
using System.Text;

namespace Pgr.AIHub.API.ChatInterface.Dummy
{
    public class AiChatMessageDummyImp : IAiChatMessage
    {
        public AiChatRole Role { get; set; }

        public string Content { get; set; } = string.Empty;
    }

    public class AiChatContextItemDummyImp : IAiChatContextItem
    {
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        public string? ContentType { get; set; }
    }

    public class AiChatOptionsDummyImp : IAiChatOptions
    {
        public string Model { get; set; } = string.Empty;

        public string Mode { get; set; } = string.Empty;

        public string? ApiKey { get; set; }
    }

    public class AiChatRequestDummyImp : IAiChatRequest
    {
        public string Prompt { get; set; } = string.Empty;

        public IReadOnlyList<IAiChatContextItem> ContextItems { get; set; } = Array.Empty<IAiChatContextItem>();

        public IAiChatOptions Options { get; set; } = new AiChatOptionsDummyImp();
    }

    public class AiChatClientWorkerDummyImp : IAiChatClientWorker
    {
        private readonly List<IAiChatMessage> messages = new();
        private bool cancelRequested;

        public event EventHandler<AiChatMessageReceivedEventArgsDummyImp>? MessageReceived;

        public IReadOnlyList<IAiChatMessage> Messages => messages;

        public Guid Guid { get; } = Guid.NewGuid();

        public async Task SendAsync(IAiChatRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            cancelRequested = false;

            messages.Add(new AiChatMessageDummyImp
            {
                Role = AiChatRole.User,
                Content = request.Prompt
            });

            await Task.Delay(100, cancellationToken);

            if (cancelRequested || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var reply = new AiChatMessageDummyImp
            {
                Role = AiChatRole.Assistant,
                Content = $"AI: {request.Prompt}"
            };

            messages.Add(reply);
            MessageReceived?.Invoke(this, new AiChatMessageReceivedEventArgsDummyImp(this, reply));
        }

        public Task SendAsync(string prompt, IAiChatOptions Options, CancellationToken cancellationToken = default)
        {
            var request = new AiChatRequestDummyImp
            {
                Prompt = prompt,
                Options = Options
            };

            return SendAsync(request, cancellationToken);
        }

        public Task CancelAsync()
        {
            cancelRequested = true;
            return Task.CompletedTask;
        }
    }

    public class AiChatManagerDummyImp : IAiChatManager
    {
        private readonly List<IAiChatClientWorker> chats = new();

        public IReadOnlyList<IAiChatClientWorker> Chats
        {
            get => chats;
            set
            {
                chats.Clear();
                if (value != null)
                {
                    chats.AddRange(value);
                }
            }
        }

        public IAiChatClientWorker CreateChat()
        {
            var chat = new AiChatClientWorkerDummyImp();
            chats.Add(chat);
            return chat;
        }
    }

    public class ChatRateLimiterDummyImp : IChatRateLimiter
    {
        private readonly Dictionary<string, DateTimeOffset> lastRequestAt = new();
        private readonly Dictionary<string, double> requestTokens = new();
        private readonly TimeSpan minimumDelay = TimeSpan.FromMilliseconds(100);

        public async Task DelayIfNeeded()
        {
            if (lastRequestAt.Count == 0)
            {
                return;
            }

            var last = DateTimeOffset.MinValue;
            foreach (var item in lastRequestAt.Values)
            {
                if (item > last)
                {
                    last = item;
                }
            }

            var delay = minimumDelay - (DateTimeOffset.UtcNow - last);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);
            }
        }

        public bool RequestStarted(string model)
        {
            lastRequestAt[model] = DateTimeOffset.UtcNow;
            return true;
        }

        public bool RequestFinished(string model, double tokens)
        {
            requestTokens[model] = tokens;
            return true;
        }
    }

    public class ModelSpecificImplementationDummyImp : IModelSepecificImplementation
    {
        private readonly HashSet<string> supportedModels = new(StringComparer.OrdinalIgnoreCase)
    {
        "GPT-5.4 mini",
        "GPT-5.4"
    };

        public IReadOnlyCollection<string> SupportedModels => supportedModels;

        public bool IsModelSupported(string model)
        {
            return supportedModels.Contains(model);
        }
    }


}
