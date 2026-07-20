using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Pgr.AIHub.API.ChatInterface;
using Pgr.AIHub.API.ChatInterface.Dummy;
using WebClient.Components.UIComponents.ChatAgentWindow;
using WebClient.Components.UIComponents.ConversationPreview;
using WebClient.Components.UIServices;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgent;

public class ChatAgentViewModel : ViewModelBase
{
    public enum ChatAgentState
    {
        Ready,
        Working,
        NoApiKey
    }

    public Guid Id { get; } = Guid.NewGuid();

    public DialogService DialogService { get; set; }
    public IAiChatManager ChatManager { get; }

    public readonly ChatAgentWindowContextViewModel Context;
    public readonly ChatAgentWindowMessageOptionsViewModel MessageOptions;
    public readonly ConversationPreviewViewModel ConversationPreview;

    private ChatAgentState state = ChatAgentState.NoApiKey;

    public ChatAgentState State
    {
        get => state;
        set
        {
            if (state == value)
            {
                return;
            }

            state = value;
            OnPropertyChanged();
        }
    }

    public List<ConversationViewModel> Conversations { get; } = new();

    private readonly List<IAiChatClientWorker> chatWorkers = new();

    private ConversationViewModel? activeChat;

    public ConversationViewModel? ActiveChat
    {
        get => activeChat;
        set
        {
            if (ReferenceEquals(activeChat, value))
            {
                return;
            }

            activeChat = value;
            OnPropertyChanged();
        }
    }

    public ChatAgentViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
        ChatManager = new AiChatManagerDummyImp();
        Context = new ChatAgentWindowContextViewModel(() => Component);
        MessageOptions = new ChatAgentWindowMessageOptionsViewModel(this, () => Component);
        ConversationPreview = new ConversationPreviewViewModel(() => Component);
        MessageOptions.PropertyChanged += MessageOptions_PropertyChanged_ApiKey;
        EnsureConversationExists();
    }

    public void UserInput_NewThread()
    {
        AddConversation();
    }

    public void UserInput_DeleteThread()
    {
        if (ActiveChat is null)
        {
            EnsureConversationExists();
            return;
        }

        if (Conversations.Count <= 1)
        {
            var oldChat = ActiveChat ?? Conversations.FirstOrDefault();
            var replacementConversation = AddConversation();
            if (oldChat != null)
            {
                RemoveConversation(oldChat);
                Conversations.Remove(oldChat);
            }

            ActiveChat = replacementConversation;
            EnsureConversationExists();
            return;
        }

        var activeChat = ActiveChat;
        RemoveConversation(activeChat);
        Conversations.Remove(activeChat);
        ActiveChat = Conversations.Count > 0 ? Conversations[0] : null;

        EnsureConversationExists();
    }

    public void UserInput_SendChatAgentMessage()
    {
    }

    public void UserInput_CancelChatAgentMessage()
    {
    }

    private void MessageOptions_PropertyChanged_ApiKey(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ChatAgentWindowMessageOptionsViewModel.ApiKey))
        {
            return;
        }

        if (State == ChatAgentState.NoApiKey && !string.IsNullOrWhiteSpace(MessageOptions.ApiKey))
        {
            State = ChatAgentState.Ready;
        }
    }

    public async Task UserInput_RenameConversation(ConversationViewModel conversation)
    {
        if (conversation == null)
        {
            return;
        }

        if (DialogService is null)
        {
            return;
        }

        var newTitle = await DialogService.ShowTextInput(
            conversation.Title,
            "Zmień nazwę konwersacji",
            "Podaj nową nazwę konwersacji");

        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            conversation.Title = newTitle;
        }
    }

    private ConversationViewModel CreateConversation()
    {
        return new ConversationViewModel(() => Component)
        {
            Title = "Nowy chat"
        };
    }

    private ConversationViewModel AddConversation()
    {
        var conversation = CreateConversation();
        Conversations.Add(conversation);
        chatWorkers.Add(ChatManager.CreateChat());
        ActiveChat = conversation;
        return conversation;
    }

    private void RemoveConversation(ConversationViewModel conversation)
    {
        var index = Conversations.FindIndex(x => x.Id == conversation.Id);
        if (index < 0)
        {
            return;
        }

        if (index < chatWorkers.Count)
        {
            ChatManager.DeleteChat(chatWorkers[index]);
            chatWorkers.RemoveAt(index);
        }
    }

    private void EnsureConversationExists()
    {
        if (Conversations.Count > 0)
        {
            if (ActiveChat is null || !Conversations.Any(x => x.Id == ActiveChat.Id))
            {
                ActiveChat = Conversations[0];
            }

            return;
        }

        AddConversation();
    }
}
