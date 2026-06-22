using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowContextViewModel : ViewModelBase
{
    public ChatAgentWindowContextViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
    }

    public string ContextText { get; set; } = "context";
}
