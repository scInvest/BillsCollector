using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.Components.UIServices;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindow : ComponentBase
{
    [Inject]
    public DialogService DialogService { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public Func<ChatAgentViewModel>? ViewModelParam { get; set; }
    public ChatAgentViewModel ViewModel => ViewModelParam?.Invoke() ?? throw new InvalidOperationException();

    private ElementReference MessageInputElement;
    private string MessageDraft = string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ViewModel.DialogService = DialogService;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ResizeMessageInputAsync();
        }
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

    private async Task UserInput_MessageDraftChanged(ChangeEventArgs args)
    {
        MessageDraft = args.Value?.ToString() ?? string.Empty;
        await ResizeMessageInputAsync();
    }

    private async Task ResizeMessageInputAsync()
    {
        await JSRuntime.InvokeVoidAsync("chatAgentWindow.autoResizeTextarea", MessageInputElement);
    }
}
