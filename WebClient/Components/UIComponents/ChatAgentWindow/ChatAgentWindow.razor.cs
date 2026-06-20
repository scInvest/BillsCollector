using Microsoft.AspNetCore.Components;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindow : ComponentBase
{
    [Parameter]
    public RenderFragment? PanelLewy { get; set; }

    private void UserInput_NewThread()
    {
    }

    private void UserInput_DeleteThread()
    {
    }
}
