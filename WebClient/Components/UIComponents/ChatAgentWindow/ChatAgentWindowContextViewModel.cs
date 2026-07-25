using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Pgr.AIHub.API.ChatInterface;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowContextViewModel : ViewModelBase
{
    public sealed record ContextModeOption(AiChatContextMode Value, string Text);

    public sealed class ContextItemViewModel
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Title { get; set; }

        public ContextItemViewModel(string title)
        {
            Title = title;
        }
    }

    private static readonly IReadOnlyList<ContextModeOption> contextModes = new[]
    {
        new ContextModeOption(AiChatContextMode.LineByLine, "Linia-po-linii"),
        new ContextModeOption(AiChatContextMode.AllAtOnce, "Wszystko naraz"),
        new ContextModeOption(AiChatContextMode.Auto, "Auto")
    };

    private AiChatContextMode selectedContextMode = contextModes[0].Value;
    private string selectedContextModeText = contextModes[0].Text;

    public ChatAgentWindowContextViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
        Contexts = new List<ContextItemViewModel>
        {
            new("context")
        };
    }

    public List<ContextItemViewModel> Contexts { get; }

    public IReadOnlyList<ContextModeOption> ContextModes => contextModes;

    public AiChatContextMode SelectedContextMode
    {
        get => selectedContextMode;
        set
        {
            if (selectedContextMode == value)
            {
                return;
            }

            selectedContextMode = value;
            selectedContextModeText = GetContextModeText(value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedContextModeText));
        }
    }

    public string SelectedContextModeText => selectedContextModeText;

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

    private static string GetContextModeText(AiChatContextMode contextMode)
    {
        foreach (var mode in contextModes)
        {
            if (mode.Value == contextMode)
            {
                return mode.Text;
            }
        }

        return contextMode.ToString();
    }
}
