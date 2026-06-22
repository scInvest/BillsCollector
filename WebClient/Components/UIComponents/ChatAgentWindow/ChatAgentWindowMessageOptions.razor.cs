using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindowMessageOptions : ComponentBase, IDisposable
{
    private ChatAgentWindowMessageOptionsViewModel? subscribedViewModel;

    [Parameter]
    public ChatAgentWindowMessageOptionsViewModel ViewModel { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ReferenceEquals(subscribedViewModel, ViewModel))
        {
            return;
        }

        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        subscribedViewModel = ViewModel;

        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
        {
            _ = InvokeAsync(StateHasChanged);
            return;
        }

        if (e.PropertyName == nameof(ChatAgentWindowMessageOptionsViewModel.SendButtonClass) ||
            e.PropertyName == nameof(ChatAgentWindowMessageOptionsViewModel.SelectedMode) ||
            e.PropertyName == nameof(ChatAgentWindowMessageOptionsViewModel.SelectedModel))
        {
            _ = InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            subscribedViewModel = null;
        }
    }
}
