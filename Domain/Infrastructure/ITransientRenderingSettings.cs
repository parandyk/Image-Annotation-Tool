using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface ITransientRenderingSettings : INotifyPropertyChanged
{
    bool GlobalBBoxVisibility { get; }
    bool GlobalClassVisibility { get; }
}