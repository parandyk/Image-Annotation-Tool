using ImageManagerViewModel = ImageAnnotationTool.ViewModels.ImageManagerViewModel;

namespace ImageAnnotationTool.Factories;

public interface IImageManagerViewModelFactory
{
    public ImageManagerViewModel Create();
}