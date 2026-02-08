using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public class AppModeSettings : ObservableObject, IAppModeSettings
{
    public AnnotationAddingMode AddingMode { get; private set; } = AnnotationAddingMode.ClickDraw;
    public bool EditingModeOn { get; private set; } = false;
    
    public void SetEditingModeOn(bool value)
    {
        if (EditingModeOn == value)
            return;
        
        EditingModeOn = value;
        OnPropertyChanged(nameof(EditingModeOn));
    }

    public void ChangeAnnotationAddingMode(AnnotationAddingMode mode)
    {
        if (AddingMode == mode)
            return;
        
        AddingMode = mode;
        OnPropertyChanged(nameof(AddingMode));
    }
}