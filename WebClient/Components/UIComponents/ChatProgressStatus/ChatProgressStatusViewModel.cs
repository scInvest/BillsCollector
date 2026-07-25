using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
    private readonly Stopwatch requestTimer = new();
    private Timer? requestTimerUpdate;

    public IReadOnlyList<IProgressStatusItem> ProgressStatuses => progressStatuses;

    public string ElapsedTime => requestTimer.Elapsed.TotalHours >= 1
        ? requestTimer.Elapsed.ToString(@"hh\:mm\:ss")
        : requestTimer.Elapsed.ToString(@"mm\:ss");

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
        requestTimerUpdate?.Dispose();
        requestTimer.Restart();
        OnPropertyChanged(nameof(ElapsedTime));
        requestTimerUpdate = new Timer(
            RequestTimerUpdate,
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1));

        AddProgressStatuses();
        CurrentStatus = RequestStatus.InProgress;
    }

    public void UserInput_StopRequest()
    {
        System_stopRequest();
    }

    private void RequestTimerUpdate(object? state)
    {
        OnPropertyChanged(nameof(ElapsedTime));
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
        requestTimerUpdate?.Dispose();
        requestTimerUpdate = null;
        requestTimer.Stop();
        OnPropertyChanged(nameof(ElapsedTime));

        if (progressStatuses.Count > 0)
        {
            progressStatuses.Clear();
            OnPropertyChanged(nameof(ProgressStatuses));
        }

        CurrentStatus = RequestStatus.Stopped;
    }
}
