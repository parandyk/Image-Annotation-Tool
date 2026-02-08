using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface IInteractionSettingsProvider : INotifyPropertyChanged
{
    double DragThreshold { get; }
    bool OverridingDefaultDragThreshold { get; }
    
    bool GlobalBBoxAnchoring { get; }
}