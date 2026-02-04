using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IAppModeSettings : IAppModeSettingsProvider
{
    void SetEditingModeOn(bool value);
    void ChangeAnnotationAddingMode(AnnotationAddingMode mode);
}