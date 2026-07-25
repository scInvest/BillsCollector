using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.Components.UIServices;

namespace WebClient.Components.UIComponents.ChatAgentWindow;

public partial class ChatAgentWindow : ComponentBase, IDisposable
{
    private ChatAgentViewModel? subscribedViewModel;
    private DotNetObjectReference<ChatAgentWindow>? dotNetReference;

    [Inject]
    public DialogService DialogService { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public Func<ChatAgentViewModel>? ViewModelParam { get; set; }
    public ChatAgentViewModel ViewModel => ViewModelParam?.Invoke() ?? throw new InvalidOperationException();

    private ElementReference MessageInputElement;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ViewModel.DialogService = DialogService;

        if (ReferenceEquals(subscribedViewModel, ViewModel))
        {
            return;
        }

        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        subscribedViewModel = ViewModel;
        subscribedViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            dotNetReference = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync(
                "chatAgentWindow.registerMessageInput",
                MessageInputElement,
                dotNetReference);
            await ResizeMessageInputAsync();
        }
    }

    [JSInvokable]
    public async Task SubmitMessageFromKeyboard()
    {
        await ViewModel.MessageOptions.UserInput_SendOrCancelChatAgentMessage();
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

        ViewModel.UserInput_SelectConversation(conversationId);
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
        ViewModel.MessageDraft = args.Value?.ToString() ?? string.Empty;
        await ResizeMessageInputAsync();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private async Task ResizeMessageInputAsync()
    {
        await JSRuntime.InvokeVoidAsync("chatAgentWindow.autoResizeTextarea", MessageInputElement);
    }

    public void Dispose()
    {
        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            subscribedViewModel = null;
        }

        dotNetReference?.Dispose();
        dotNetReference = null;
    }
}
