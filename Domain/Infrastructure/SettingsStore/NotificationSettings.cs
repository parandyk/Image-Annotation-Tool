using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public class NotificationSettings : ObservableObject, INotificationSettings
{
    public bool SuppressLocalClassInstanceDeletionDialogs { get; private set; } = false;
    public bool SuppressGlobalClassInstanceDeletionDialogs { get; private set; } = false;
    public bool SuppressAnnotationDeletionDialogs { get; private set; } = false;
    // public bool SuppressLocalClassInstanceSwapDialogs { get; private set; } = false;
    // public bool SuppressGlobalClassInstanceSwapDialogs { get; private set; } = false;
    public bool SuppressClassDeletionDialogs { get; private set; } = false;
    public bool SuppressImageDeletionDialogs { get; private set; } = false;

    public void ChangeSuppressionLocalClassInstanceDeletionDialogs(bool state)
    {
        if (SuppressLocalClassInstanceDeletionDialogs == state)
            return;
        
        SuppressLocalClassInstanceDeletionDialogs = state;
        OnPropertyChanged(nameof(SuppressLocalClassInstanceDeletionDialogs));
    }

    public void ChangeSuppressionGlobalClassInstanceDeletionDialogs(bool state)
    {
        if (SuppressGlobalClassInstanceDeletionDialogs == state)
            return;
        
        SuppressGlobalClassInstanceDeletionDialogs = state;
        OnPropertyChanged(nameof(SuppressGlobalClassInstanceDeletionDialogs));
    }

    public void ChangeSuppressionAnnotationDeletionDialogs(bool state)
    {
        if (SuppressAnnotationDeletionDialogs == state)
            return;
        
        SuppressAnnotationDeletionDialogs = state;
        OnPropertyChanged(nameof(SuppressAnnotationDeletionDialogs));
    }

    // public void ChangeSuppressionLocalClassInstanceSwapDialogs(bool state)
    // {
    //     if (SuppressLocalClassInstanceSwapDialogs == state)
    //         return;
    //     
    //     SuppressLocalClassInstanceSwapDialogs = state;
    //     OnPropertyChanged(nameof(SuppressLocalClassInstanceSwapDialogs));
    // }
    //
    // public void ChangeSuppressionGlobalClassInstanceSwapDialogs(bool state)
    // {
    //     if (SuppressGlobalClassInstanceSwapDialogs == state)
    //         return;
    //     
    //     SuppressGlobalClassInstanceSwapDialogs = state;
    //     OnPropertyChanged(nameof(SuppressGlobalClassInstanceSwapDialogs));
    // }

    public void ChangeSuppressionClassDeletionDialogs(bool state)
    {
        if (SuppressClassDeletionDialogs == state)
            return;
        
        SuppressClassDeletionDialogs = state;
        OnPropertyChanged(nameof(SuppressClassDeletionDialogs));
    }

    public void ChangeSuppressionImageDeletionDialogs(bool state)
    {
        if (SuppressImageDeletionDialogs == state)
            return;
        
        SuppressImageDeletionDialogs = state;
        OnPropertyChanged(nameof(SuppressImageDeletionDialogs));
    }
}