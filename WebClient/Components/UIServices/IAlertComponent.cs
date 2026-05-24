namespace WebClient.Components.UIServices
{
    // Interface implemented by a component that can show alerts (with optional title)
    // and ask for confirmation (yes/no) with optional title.
    public interface IAlertComponent
    {
        Task ShowAlert(string text);
        Task ShowAlert(string text, string? title);

        Task<bool> Confirm(string text);
        Task<bool> Confirm(string text, string? title);
    }
}
