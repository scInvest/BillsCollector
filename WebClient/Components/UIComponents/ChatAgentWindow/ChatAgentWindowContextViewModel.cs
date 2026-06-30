using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowContextViewModel : ViewModelBase
{
    public sealed class ContextItemViewModel
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Title { get; set; }

        public ContextItemViewModel(string title)
        {
            Title = title;
        }
    }

    public ChatAgentWindowContextViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
        Contexts = new List<ContextItemViewModel>
        {
            new("context")
        };
    }

    public List<ContextItemViewModel> Contexts { get; }

    public void UserInput_AddContext()
    {
        Contexts.Add(new("context"));
        OnPropertyChanged(nameof(Contexts));
    }

    public void UserInput_RemoveContext(ContextItemViewModel context)
    {
        if (Contexts.Remove(context))
        {
            OnPropertyChanged(nameof(Contexts));
        }
    }
}
