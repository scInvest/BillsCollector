using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using WebClient.Components.UIComponents.ChatAgent;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindowMessageOptions : ComponentBase, IDisposable
{
    private ChatAgentViewModel? subscribedParent;

    [Parameter]
    public ChatAgentWindowMessageOptionsViewModel ViewModel { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ReferenceEquals(subscribedParent, ViewModel.ParentViewModel))
        {
            return;
        }

        if (subscribedParent is not null)
        {
            subscribedParent.PropertyChanged -= ParentViewModel_PropertyChanged;
        }

        subscribedParent = ViewModel.ParentViewModel;

        if (subscribedParent is not null)
        {
            subscribedParent.PropertyChanged += ParentViewModel_PropertyChanged;
        }
    }

    private void ParentViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null || e.PropertyName == nameof(ChatAgentViewModel.State))
        {
            _ = InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        if (subscribedParent is not null)
        {
            subscribedParent.PropertyChanged -= ParentViewModel_PropertyChanged;
            subscribedParent = null;
        }
    }
}
