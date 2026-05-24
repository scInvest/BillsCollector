namespace WebClient.Components.UIServices
{
    public class DialogService
    {
        // Handler registered by a component that knows how to show a JS alert.
        // It should accept a string and return a Task.
        private Func<string, Task>? _alertHandler;

        // Register the alert handler (called by the alert component on initialization)
        public void RegisterAlertHandler(Func<string, Task> handler)
        {
            _alertHandler = handler;
        }

        // Unregister the alert handler (called by the alert component on dispose)
        public void UnregisterAlertHandler()
        {
            _alertHandler = null;
        }

        // Show an alert using the registered handler if available. No-op if none registered.
        public async Task ShowAlert(string text)
        {
            if (_alertHandler != null)
            {
                await _alertHandler.Invoke(text);
            }
            else
            {
                // No registered component to show JS alert. Silently ignore or consider logging.
                await Task.CompletedTask;
            }
        }
    }
}
