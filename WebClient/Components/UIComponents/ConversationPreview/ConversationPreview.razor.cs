using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace WebClient.Components.UIComponents.ConversationPreview;

public partial class ConversationPreview : ComponentBase, IDisposable
{
    private ConversationPreviewViewModel? subscribedViewModel;

    [Parameter]
    public ConversationPreviewViewModel ViewModel { get; set; } = default!;

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

    private static string GetMessageCssClass(ConversationPreviewViewModel.ConversationMessageViewModel message)
    {
        return message.Author == ConversationPreviewViewModel.MessageAuthor.User
            ? "conversation-preview__message--user"
            : "conversation-preview__message--system";
    }

    private static string GetMessageTooltip(ConversationPreviewViewModel.ConversationMessageViewModel message)
    {
        return message.Date.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
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
