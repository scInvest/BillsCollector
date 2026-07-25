using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.ChatProgressStatus;

public class ChatProgressStatusViewModel : ViewModelBase
{
    public interface IProgressStatusItem
    {
        string Message { get; }

        string ProgressState { get; }

        string GetDisplayText()
        {
            return string.IsNullOrWhiteSpace(ProgressState)
                ? Message
                : $"{ProgressState}: {Message}";
        }
    }

    public sealed record ProgressStatusItem(string Message, string ProgressState) : IProgressStatusItem;

    public enum RequestStatus
    {
        Stopped,
        InProgress
    }

    private readonly List<IProgressStatusItem> progressStatuses = new();

    public IReadOnlyList<IProgressStatusItem> ProgressStatuses => progressStatuses;

    private RequestStatus currentStatus = RequestStatus.Stopped;

    public ChatProgressStatusViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
    }

    public RequestStatus CurrentStatus
    {
        get => currentStatus;
        private set
        {
            if (currentStatus == value)
            {
                return;
            }

            currentStatus = value;
            OnPropertyChanged();
        }
    }

    public void UserInput_StartRequest()
    {
        AddProgressStatuses();
        CurrentStatus = RequestStatus.InProgress;
    }

    public void UserInput_StopRequest()
    {
        System_stopRequest();
    }

    public void System_progressUpdate()
    {
        AddProgressStatuses();
        CurrentStatus = RequestStatus.InProgress;
    }

    private void AddProgressStatuses()
    {
        if (progressStatuses.Count == 0)
        {
            progressStatuses.Add(new ProgressStatusItem("Czekamy na odpowiedź", "w toku"));
            progressStatuses.Add(new ProgressStatusItem("Przygotowujemy odpowiedź", "oczekiwanie"));
            OnPropertyChanged(nameof(ProgressStatuses));
        }
    }

    public void System_stopRequest()
    {
        if (progressStatuses.Count > 0)
        {
            progressStatuses.Clear();
            OnPropertyChanged(nameof(ProgressStatuses));
        }

        CurrentStatus = RequestStatus.Stopped;
    }
}
