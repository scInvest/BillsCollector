using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using WebClient.Components.UIComponents.ChatAgentWindow;
using WebClient.Components.UIServices;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgent;

public class ChatAgentViewModel : ViewModelBase
{
    public enum ChatAgentState
    {
        Ready,
        Working
    }

    public Guid Id { get; } = Guid.NewGuid();

    public DialogService? DialogService { get; set; }

    public readonly ChatAgentWindowContextViewModel Context;
    public readonly ChatAgentWindowMessageOptionsViewModel MessageOptions;

    private ChatAgentState state = ChatAgentState.Ready;

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
        Context = new ChatAgentWindowContextViewModel(() => Component);
        MessageOptions = new ChatAgentWindowMessageOptionsViewModel(this, () => Component);
        EnsureConversationExists();
    }

    public void UserInput_NewThread()
    {
        var conversation = CreateConversation();

        Conversations.Add(conversation);
        ActiveChat = conversation;
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
            var replacementConversation = CreateConversation();
            Conversations.Add(replacementConversation);
            if (oldChat != null)
            {
                Conversations.Remove(oldChat);
            }

            ActiveChat = replacementConversation;
            EnsureConversationExists();
            return;
        }

        Conversations.Remove(ActiveChat);
        ActiveChat = Conversations.Count > 0 ? Conversations[0] : null;

        EnsureConversationExists();
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
            "Zmie? nazw? konwersacji",
            "Podaj now? nazw? konwersacji");

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

        var conversation = CreateConversation();
        Conversations.Add(conversation);
        ActiveChat = conversation;
    }
}
