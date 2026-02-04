using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IPersistentRenderingSettings : INotifyPropertyChanged
{
    bool OverridingDefaultBboxBorderThickness { get; }
    double BboxBorderThickness { get; }
    bool BboxBackgroundOn { get; }
    bool BboxBorderOn { get; }
    bool DynamicBordersOn { get; }
}