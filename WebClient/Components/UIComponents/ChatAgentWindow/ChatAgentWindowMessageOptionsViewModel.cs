using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.ComponentModel;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowMessageOptionsViewModel : ViewModelBase
{
    public sealed record ModeOption(string Value, string Tooltip);

    private static readonly IReadOnlyList<ModeOption> modes = new[]
    {
        new ModeOption("Agent", "Tryb agenta — może analizować i zmieniać dane"),
        new ModeOption("Pytanie", "Tryb rozmowy — zwykły czat, bez zmian w danych"),
    };
    private static readonly IReadOnlyList<string> models = new[] { "GPT-5.4 mini", "GPT-5.4" };

    private readonly ChatAgentViewModel parentViewModel;

    private string selectedMode = modes[0].Value;
    private string selectedModel = models[0];
    private string? apiKey;
    private string sendButtonClass = "is-ready";

    public ChatAgentWindowMessageOptionsViewModel(ChatAgentViewModel parentViewModel, Func<ComponentBase> getComponent)
        : base(getComponent)
    {
        this.parentViewModel = parentViewModel;
        this.parentViewModel.PropertyChanged += ParentViewModel_PropertyChanged;
        UpdateSendButtonClass();
    }

    public IReadOnlyList<ModeOption> Modes => modes;

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

    public string? ApiKey
    {
        get => apiKey;
        set
        {
            if (apiKey == value)
            {
                return;
            }

            apiKey = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ApiKeyButtonClass));
        }
    }

    public string SendButtonClass => sendButtonClass;

    public string ApiKeyButtonClass => string.IsNullOrEmpty(apiKey) ? "is-missing" : "is-present";

    public void UserInput_SendOrCancelChatAgentMessage()
    {
        if (parentViewModel.State == ChatAgentViewModel.ChatAgentState.Ready)
        {
            parentViewModel.UserInput_SendChatAgentMessage();
        }
        else if (parentViewModel.State == ChatAgentViewModel.ChatAgentState.Working)
        {
            parentViewModel.UserInput_CancelChatAgentMessage();
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private void ParentViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null || e.PropertyName == nameof(ChatAgentViewModel.State))
        {
            UpdateSendButtonClass();
        }
    }

    private void UpdateSendButtonClass()
    {
        var nextClass = parentViewModel.State == ChatAgentViewModel.ChatAgentState.Ready
            ? "is-ready"
            : "is-working";

        if (sendButtonClass == nextClass)
        {
            return;
        }

        sendButtonClass = nextClass;
        OnPropertyChanged(nameof(SendButtonClass));
    }
}
