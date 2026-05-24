namespace WebClient.Components.UIServices
{
    public class DialogService
    {
        // Handler registered by a component that knows how to show an alert with optional title.
        // It should accept text and optional title and return a Task.
        private Func<string, string?, Task>? _alertHandler;

        // Register the alert handler (called by the alert component on initialization)
        public void RegisterAlertHandler(Func<string, string?, Task> handler)
        {
            _alertHandler = handler;
        }

        // Unregister the alert handler (called by the alert component on dispose)
        public void UnregisterAlertHandler()
        {
            _alertHandler = null;
        }

        // Show an alert using the registered handler if available. No-op if none registered.
        public Task ShowAlert(string text) => ShowAlert(text, null);

        // Overload allowing title to be provided.
        public async Task ShowAlert(string text, string? title)
        {
            if (_alertHandler != null)
            {
                await _alertHandler.Invoke(text, title);
            }
            else
            {
                // No registered component to show JS alert. Silently ignore or consider logging.
                await Task.CompletedTask;
            }
        }
    }
}
