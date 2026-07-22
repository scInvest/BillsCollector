using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.ViewModels;
using MudBlazor;

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

    public async Task UserInput_AddApiKey()
    {
        var key = await this.parentViewModel.DialogService.ShowTextInput("Dodaj klucz API", "Dodaj klucz API", "Wprowadź swój klucz API, aby korzystać z AI.");
        if (key != null)
        {
            this.ApiKey = key;
        }
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
            UpdateParentStateFromApiKey();
        }
    }

    public string SendButtonClass => sendButtonClass;

    public string SendButtonTooltip => parentViewModel.State == ChatAgentViewModel.ChatAgentState.Ready
        ? "Wyślij wiadomość"
        : "Zatrzymaj czat";

    public string ApiKeyButtonClass => string.IsNullOrEmpty(apiKey) ? "is-missing" : "is-present";

    public async Task UserInput_SendOrCancelChatAgentMessage()
    {
        if (parentViewModel.State == ChatAgentViewModel.ChatAgentState.Ready)
        {
            await parentViewModel.UserInput_SendChatAgentMessage();
        }
        else if (parentViewModel.State == ChatAgentViewModel.ChatAgentState.Working)
        {
            await parentViewModel.UserInput_CancelChatAgentMessage();
        }
        else
        {
            await parentViewModel.DialogService.ShowAlert("Dodaj klucz API, aby korzystać z AI. Instrukcja znajduje sie po prawej stronie.");
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
        var nextClass = parentViewModel.State == ChatAgentViewModel.ChatAgentState.Working
            ? "is-working"
            : "is-ready";

        if (sendButtonClass == nextClass)
        {
            return;
        }

        sendButtonClass = nextClass;
        OnPropertyChanged(nameof(SendButtonClass));
    }

    private void UpdateParentStateFromApiKey()
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            parentViewModel.State = ChatAgentViewModel.ChatAgentState.NoApiKey;
        }
        else if (parentViewModel.State == ChatAgentViewModel.ChatAgentState.NoApiKey)
        {
            parentViewModel.State = ChatAgentViewModel.ChatAgentState.Ready;
        }
    }
}
