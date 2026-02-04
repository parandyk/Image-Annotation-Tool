using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface ISettingsManagerViewModelFactory
{
    public SettingsManagerViewModel Create();
}