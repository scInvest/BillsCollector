using Microsoft.AspNetCore.Components;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindowMessageOptions : ComponentBase
{
    [Parameter]
    public ChatAgentWindowMessageOptionsViewModel ViewModel { get; set; } = default!;
}
