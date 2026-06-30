using Microsoft.AspNetCore.Components;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindowContext : ComponentBase
{
    [Parameter]
    public ChatAgentWindowContextViewModel ViewModel { get; set; } = default!;

    private void UserInput_AddContext()
    {
        ViewModel?.UserInput_AddContext();
    }

    private void UserInput_RemoveContext(ChatAgentWindowContextViewModel.ContextItemViewModel context)
    {
        ViewModel?.UserInput_RemoveContext(context);
    }
}
