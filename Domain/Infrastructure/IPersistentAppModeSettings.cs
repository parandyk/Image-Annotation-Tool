using System.ComponentModel;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IPersistentAppModeSettings : INotifyPropertyChanged
{
    AnnotationAddingMode AddingMode { get; }
}