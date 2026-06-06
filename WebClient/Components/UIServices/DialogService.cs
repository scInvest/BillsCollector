namespace WebClient.Components.UIServices
{
    public class DialogService
    {
        private IAlertComponent? _alertComponent;
        private ITextInputComponent? _textInputComponent;

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

        // Text input component registration
        public void RegisterTextInputComponent(ITextInputComponent component)
        {
            _textInputComponent = component;
        }

        public void UnregisterTextInputComponent()
        {
            _textInputComponent = null;
        }

        // Show the text input modal and return entered text or null if cancelled
        public async Task<string?> ShowTextInput(string initialText = "", string? title = null, string hintText = "")
        {
            if (_textInputComponent != null)
            {
                return await _textInputComponent.ShowAsync(initialText, title, hintText);
            }

            return null;
        }
    }
}
