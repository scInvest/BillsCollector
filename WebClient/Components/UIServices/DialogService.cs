namespace WebClient.Components.UIServices
{
    public class DialogService
    {
        private IAlertComponent? _alertComponent;

        // Register the alert component (called by the alert component on initialization)
        public void RegisterAlertComponent(IAlertComponent component)
        {
            _alertComponent = component;
        }

        // Unregister the alert component (called by the alert component on dispose)
        public void UnregisterAlertComponent()
        {
            _alertComponent = null;
        }

        // Show an alert using the registered component if available. No-op if none registered.
        public Task ShowAlert(string text) => ShowAlert(text, null);

        // Overload allowing title to be provided.
        public async Task ShowAlert(string text, string? title)
        {
            if (_alertComponent != null)
            {
                await _alertComponent.ShowAlert(text, title);
            }
            else
            {
                await Task.CompletedTask;
            }
        }

        // Ask a yes/no question. Defaults to false if no component registered.
        public Task<bool> Confirm(string text) => Confirm(text, null);

        public async Task<bool> Confirm(string text, string? title)
        {
            if (_alertComponent != null)
            {
                return await _alertComponent.Confirm(text, title);
            }

            return false;
        }
    }
}
