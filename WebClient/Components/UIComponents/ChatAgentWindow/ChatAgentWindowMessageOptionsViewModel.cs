using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowMessageOptionsViewModel : ViewModelBase
{
    private static readonly IReadOnlyList<string> modes = new[] { "Agent", "Pytanie" };
    private static readonly IReadOnlyList<string> models = new[] { "GPT-5.4 mini", "GPT-5.4" };

    private string selectedMode = modes[0];
    private string selectedModel = models[0];

    public ChatAgentWindowMessageOptionsViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
    }

    public IReadOnlyList<string> Modes => modes;

    public IReadOnlyList<string> Models => models;

    public string SelectedMode
    {
        get => selectedMode;
        set
        {
            if (selectedMode == value)
            {
                return;
            }

            selectedMode = value;
            OnPropertyChanged();
        }
    }

    public string SelectedModel
    {
        get => selectedModel;
        set
        {
            if (selectedModel == value)
            {
                return;
            }

            selectedModel = value;
            OnPropertyChanged();
        }
    }
}
