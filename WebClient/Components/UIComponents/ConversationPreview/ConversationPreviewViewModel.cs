using System;
using System.Collections.Generic;
using System.Linq;
using Pgr.AIHub.API.ChatInterface;
using Microsoft.AspNetCore.Components;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.Components.UIComponents.ChatProgressStatus;
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

    private ConversationViewModel? activeChat;
    private IAiChatClientWorker? subscribedChat;

    public ConversationViewModel? ActiveChat
    {
        get => activeChat;
        set
        {
            if (ReferenceEquals(activeChat, value))
            {
                return;
            }

            UnsubscribeFromActiveChat();
            activeChat = value;
            SubscribeToActiveChat();
            OnPropertyChanged();
            OnPropertyChanged(nameof(Messages));
        }
    }

    public IReadOnlyList<ConversationMessageViewModel> Messages
    {
        get
        {
            return activeChat?.Chat.Messages
                .Select(MapMessage)
                .ToArray() ?? Array.Empty<ConversationMessageViewModel>();
        }
    }

    public ChatProgressStatusViewModel ProgressStatus { get; }

    public ConversationPreviewViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
        ProgressStatus = new ChatProgressStatusViewModel(() => Component);
    }

    public void UserInput_StartRequest()
    {
        ProgressStatus.UserInput_StartRequest();
    }

    public void UserInput_StopRequest()
    {
        ProgressStatus.UserInput_StopRequest();
    }

    public void UserInput_RefreshMessages()
    {
        OnPropertyChanged(nameof(Messages));
    }

    private static ConversationMessageViewModel MapMessage(IAiChatMessage message)
    {
        return new ConversationMessageViewModel(
            DateTimeOffset.UtcNow,
            message.Content,
            message.Role == AiChatRole.User ? MessageAuthor.User : MessageAuthor.System);
    }

    private void SubscribeToActiveChat()
    {
        if (activeChat?.Chat is null)
        {
            return;
        }

        subscribedChat = activeChat.Chat;
        subscribedChat.MessageReceived += ActiveChat_MessageReceived;
        subscribedChat.ChatStopped += ActiveChat_ChatStopped;
    }

    private void UnsubscribeFromActiveChat()
    {
        if (subscribedChat is null)
        {
            return;
        }

        subscribedChat.MessageReceived -= ActiveChat_MessageReceived;
        subscribedChat.ChatStopped -= ActiveChat_ChatStopped;
        subscribedChat = null;
    }

    private void ActiveChat_MessageReceived(object? sender, AiChatMessageReceivedEventArgsDummyImp e)
    {
        UserInput_RefreshMessages();
    }

    private void ActiveChat_ChatStopped(object? sender, EventArgs e)
    {
        ProgressStatus.System_stopRequest();
    }
}
