using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface ISettingsDialogWrapper
{
    SettingsDialogViewModel CreateSettingsDialog();
}