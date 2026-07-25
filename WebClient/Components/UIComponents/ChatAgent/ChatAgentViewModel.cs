using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
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

    private ConversationViewModel? activeChat;
    private string messageDraft = string.Empty;

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
            ConversationPreview.ActiveChat = activeChat;
            OnPropertyChanged();
        }
    }

    public string MessageDraft
    {
        get => messageDraft;
        set
        {
            if (messageDraft == value)
            {
                return;
            }

            messageDraft = value;
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

    public void UserInput_SelectConversation(Guid conversationId)
    {
        var conversation = Conversations.FirstOrDefault(x => x.Id == conversationId);
        if (conversation is not null)
        {
            ActiveChat = conversation;
        }
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

    public async Task UserInput_SendChatAgentMessage()
    {
        if (State != ChatAgentState.Ready ||
            ActiveChat is null ||
            string.IsNullOrWhiteSpace(MessageOptions.ApiKey) ||
            string.IsNullOrWhiteSpace(MessageDraft))
        {
            return;
        }

        var chat = ActiveChat.Chat;
        var request = new AiChatRequestDummyImp
        {
            Prompt = MessageDraft,
            Options = new AiChatOptionsDummyImp
            {
                ApiKey = MessageOptions.ApiKey,
                Model = MessageOptions.SelectedModel,
                Mode = MessageOptions.SelectedMode
            },
            ContextItems = Context.Contexts
                .Select(context => new AiChatContextItemDummyImp
                {
                    Title = context.Title
                })
                .ToArray(),
            ContextMode = Context.SelectedContextMode
        };

        State = ChatAgentState.Working;
        ConversationPreview.UserInput_StartRequest();
        MessageDraft = string.Empty;

        try
        {
            var sendTask = chat.SendAsync(request);
            ConversationPreview.UserInput_RefreshMessages();
            await sendTask;
        }
        finally
        {
            ConversationPreview.UserInput_StopRequest();
            State = string.IsNullOrWhiteSpace(MessageOptions.ApiKey)
                ? ChatAgentState.NoApiKey
                : ChatAgentState.Ready;
        }
    }

    public async Task UserInput_CancelChatAgentMessage()
    {
        if (State != ChatAgentState.Working || ActiveChat is null)
        {
            return;
        }

        await ActiveChat.Chat.CancelAsync();
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
        var chat = ChatManager.CreateChat();

        return new ConversationViewModel(() => Component, chat)
        {
            Title = "Nowy chat"
        };
    }

    private ConversationViewModel AddConversation()
    {
        var conversation = CreateConversation();
        Conversations.Add(conversation);
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

        if (conversation.Chat is not null)
        {
            ChatManager.DeleteChat(conversation.Chat);
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
