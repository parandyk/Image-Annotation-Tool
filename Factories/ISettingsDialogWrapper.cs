using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Factories;

public interface ISettingsDialogWrapper
{
    SettingsDialogViewModel CreateSettingsDialog();
}