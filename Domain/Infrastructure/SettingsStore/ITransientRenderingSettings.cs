using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface ITransientRenderingSettings : INotifyPropertyChanged
{
    bool GlobalAnnotationVisibility { get; }
    bool GlobalClassVisibility { get; }
    bool FilteredAnnotationVisibility { get; }
    bool FilteredClassVisibility { get; }
}