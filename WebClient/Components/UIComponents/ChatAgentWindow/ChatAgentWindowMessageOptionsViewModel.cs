using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public class ChatAgentWindowMessageOptionsViewModel : ViewModelBase
{
    public ChatAgentWindowMessageOptionsViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
    }

    public string PlaceholderText { get; set; } = "Lorem ipsum dolor sit amet.";
}
