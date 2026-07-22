using System;
using Pgr.AIHub.API.ChatInterface;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgent;

public class ConversationViewModel : ViewModelBase
{
    public Guid Id { get; } = Guid.NewGuid();

    public IAiChatClientWorker Chat { get; }

    private string title = string.Empty;

    public event Action<string>? UserInput_TitleChanged;

    public ConversationViewModel(Func<ComponentBase> getComponent, IAiChatClientWorker chat)
        : base(getComponent)
    {
        Chat = chat;
    }

    public string Title
    {
        get => title;
        set
        {
            if (title == value)
            {
                return;
            }

            title = value;
            OnPropertyChanged();
            UserInput_TitleChanged?.Invoke(nameof(Title));
        }
    }
}