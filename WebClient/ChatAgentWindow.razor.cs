using Microsoft.AspNetCore.Components;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.Components.UIServices;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindow : ComponentBase
{
    [Inject]
    public DialogService DialogService { get; set; } = default!;

    [Parameter]
    public RenderFragment? PanelLewy { get; set; }

    [Parameter]
    public Func<ChatAgentViewModel>? ViewModelParam { get; set; }
    public ChatAgentViewModel ViewModel => ViewModelParam?.Invoke() ?? throw new InvalidOperationException();

    private void UserInput_NewThread()
    {
        ViewModel?.UserInput_NewThread();
    }

    private void UserInput_DeleteThread()
    {
        ViewModel?.UserInput_DeleteThread();
    }

    private void UserInput_RenameConversation(ConversationViewModel conversation)
    {
        ViewModel?.UserInput_RenameConversation(conversation);
    }
}
