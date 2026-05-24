namespace WebClient.Components.UIServices
{
    // Interface implemented by a component that can show alerts (with optional title).
    public interface IAlertComponent
    {
        Task ShowAlert(string text);
        Task ShowAlert(string text, string? title);
    }
}
