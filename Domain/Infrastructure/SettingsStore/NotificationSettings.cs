using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public class NotificationSettings : ObservableObject, INotificationSettings
{
    public bool SuppressLocalClassInstanceDeletionDialogs { get; private set; } = false;
    public bool SuppressGlobalClassInstanceDeletionDialogs { get; private set; } = false;
    public bool SuppressAnnotationDeletionDialogs { get; private set; } = false;
    public bool SuppressClassDeletionDialogs { get; private set; } = false;
    public bool SuppressImageDeletionDialogs { get; private set; } = false;
    public bool SuppressUnassignedExportWarningDialog { get; private set; } = false;

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

    public void ChangeSuppressionUnassignedExportWarningDialog(bool state)
    {
        if (SuppressUnassignedExportWarningDialog == state)
            return;
        
        SuppressUnassignedExportWarningDialog = state;
        OnPropertyChanged(nameof(SuppressUnassignedExportWarningDialog));
    }
}