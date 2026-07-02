using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace WebClient.Components.UIComponents.ChatProgressStatus;

public partial class ChatProgressStatus : ComponentBase, IDisposable
{
    private ChatProgressStatusViewModel? subscribedViewModel;

    [Parameter]
    public ChatProgressStatusViewModel ViewModel { get; set; } = default!;

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
        _ = InvokeAsync(StateHasChanged);
    }

    private string GetStatusClass()
    {
        return ViewModel.CurrentStatus == ChatProgressStatusViewModel.RequestStatus.InProgress
            ? "chat-progress-status--in-progress"
            : "chat-progress-status--stopped";
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
