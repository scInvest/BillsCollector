using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ConversationPreview;

public class ConversationPreviewViewModel : ViewModelBase
{
    public enum MessageAuthor
    {
        System,
        User
    }

    public sealed class ConversationMessageViewModel
    {
        public ConversationMessageViewModel(DateTimeOffset date, string content, MessageAuthor author)
        {
            Date = date;
            Content = content;
            Author = author;
        }

        public DateTimeOffset Date { get; }

        public string Content { get; }

        public MessageAuthor Author { get; }
    }

    public IReadOnlyList<ConversationMessageViewModel> Messages { get; } = new[]
    {
        new ConversationMessageViewModel(DateTimeOffset.UtcNow.AddMinutes(-3), "_chat_placeholder", MessageAuthor.System),
        new ConversationMessageViewModel(DateTimeOffset.UtcNow.AddMinutes(-2), "_chat_placeholder", MessageAuthor.User),
        new ConversationMessageViewModel(DateTimeOffset.UtcNow.AddMinutes(-1), "_chat_placeholder", MessageAuthor.System)
    };

    public ConversationPreviewViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
    }
}
