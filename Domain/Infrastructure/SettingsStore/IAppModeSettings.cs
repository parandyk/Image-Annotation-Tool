using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface IAppModeSettings : IAppModeSettingsProvider
{
    void SetEditingModeOn(bool value);
    void ChangeAnnotationAddingMode(AnnotationAddingMode mode);
}