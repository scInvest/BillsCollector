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

    public IReadOnlyList<IProgressStatusItem> ProgressStatuses { get; }

    private RequestStatus currentStatus = RequestStatus.Stopped;

    public ChatProgressStatusViewModel(Func<ComponentBase> getComponent)
        : base(getComponent)
    {
        ProgressStatuses = new IProgressStatusItem[]
        {
            new ProgressStatusItem("progress_placeholder", "in progress"),
            new ProgressStatusItem("progress_placeholder", "waiting")
        };
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
        CurrentStatus = RequestStatus.InProgress;
    }

    public void UserInput_StopRequest()
    {
        CurrentStatus = RequestStatus.Stopped;
    }

    public void System_progressUpdate()
    {
        CurrentStatus = RequestStatus.InProgress;
    }

    public void System_stopRequest()
    {
        CurrentStatus = RequestStatus.Stopped;
    }
}
