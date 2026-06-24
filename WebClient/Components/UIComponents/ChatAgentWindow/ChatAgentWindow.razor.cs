using Microsoft.AspNetCore.Components;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.Components.UIServices;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindow : ComponentBase
{
    [Inject]
    public DialogService DialogService { get; set; } = default!;

    [Parameter]
    public Func<ChatAgentViewModel>? ViewModelParam { get; set; }
    public ChatAgentViewModel ViewModel => ViewModelParam?.Invoke() ?? throw new InvalidOperationException();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ViewModel.DialogService = DialogService;
    }

    private void UserInput_NewThread()
    {
        ViewModel?.UserInput_NewThread();
    }

    private void UserInput_DeleteThread()
    {
        ViewModel?.UserInput_DeleteThread();
    }

    private void UserInput_SelectConversation(ChangeEventArgs args)
    {
        if (ViewModel is null || args.Value is null)
        {
            return;
        }

        if (!Guid.TryParse(args.Value.ToString(), out var conversationId))
        {
            return;
        }

        foreach (var conversation in ViewModel.Conversations)
        {
            if (conversation.Id == conversationId)
            {
                ViewModel.ActiveChat = conversation;
                return;
            }
        }
    }

    private async Task UserInput_RenameConversation(ConversationViewModel? conversation)
    {
        if (ViewModel is null || conversation is null)
        {
            return;
        }

        await ViewModel.UserInput_RenameConversation(conversation);
    }
}
