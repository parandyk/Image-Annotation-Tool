using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface INotificationSettingsProvider : INotifyPropertyChanged
{
    
    bool SuppressLocalClassInstanceDeletionDialogs { get; }
    bool SuppressGlobalClassInstanceDeletionDialogs { get; }
    bool SuppressAnnotationDeletionDialogs{ get; }
    // bool SuppressLocalClassInstanceSwapDialogs { get; }
    // bool SuppressGlobalClassInstanceSwapDialogs { get; }
    bool SuppressClassDeletionDialogs { get; }
    bool SuppressImageDeletionDialogs { get; }
}