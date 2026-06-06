namespace WebClient.Components.UIServices
{
    // Interface for a component that can show a text input modal and return the entered string (or null if cancelled).
    public interface ITextInputComponent
    {
        Task<string?> ShowAsync(string initialText = "", string? title = null, string hintText = "");
    }
}
