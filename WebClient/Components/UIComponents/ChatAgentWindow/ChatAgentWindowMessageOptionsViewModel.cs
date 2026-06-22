using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowMessageOptionsViewModel : ViewModelBase
{
    public ChatAgentWindowMessageOptionsViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
    }

    public IReadOnlyList<string> Modes { get; } = new[] { "Agent", "Pytanie" };

    public IReadOnlyList<string> Models { get; } = new[] { "GPT-5.4 mini", "GPT-5.4" };

    public string SelectedMode { get; set; } = "Agent";

    public string SelectedModel { get; set; } = "GPT-5.4 mini";
}
