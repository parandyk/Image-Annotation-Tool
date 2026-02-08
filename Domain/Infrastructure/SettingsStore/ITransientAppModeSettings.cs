using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface ITransientAppModeSettings : INotifyPropertyChanged
{ 
    bool EditingModeOn { get; }
}