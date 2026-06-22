using Microsoft.AspNetCore.Components;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindowContext : ComponentBase
{
    [Parameter]
    public ChatAgentWindowContextViewModel ViewModel { get; set; } = default!;
}
