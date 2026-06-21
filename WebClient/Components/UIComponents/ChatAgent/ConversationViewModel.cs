using System;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgent;

public class ConversationViewModel : ViewModelBase
{
    public Guid Id { get; } = Guid.NewGuid();

    private string title = string.Empty;

    public event Action<string>? UserInput_PropertyChanged;

    public ConversationViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
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
            UserInput_PropertyChanged?.Invoke(nameof(Title));
        }
    }
}