using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface ITransientAppModeSettings : INotifyPropertyChanged
{ 
    bool EditingModeOn { get; }
}