using System.ComponentModel;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface IPersistentAppModeSettings : INotifyPropertyChanged
{
    AnnotationAddingMode AddingMode { get; }
}