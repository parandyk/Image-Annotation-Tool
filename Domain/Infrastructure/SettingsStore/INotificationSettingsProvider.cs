using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface INotificationSettingsProvider : INotifyPropertyChanged
{
    
    bool SuppressLocalClassInstanceDeletionDialogs { get; }
    bool SuppressGlobalClassInstanceDeletionDialogs { get; }
    bool SuppressAnnotationDeletionDialogs{ get; }
    bool SuppressClassDeletionDialogs { get; }
    bool SuppressImageDeletionDialogs { get; }
    bool SuppressUnassignedExportWarningDialog { get; }
}