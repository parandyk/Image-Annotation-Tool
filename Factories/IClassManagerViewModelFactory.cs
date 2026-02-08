using ClassManagerViewModel = ImageAnnotationTool.ViewModels.ClassManagerViewModel;

namespace ImageAnnotationTool.Factories;

public interface IClassManagerViewModelFactory
{
    public ClassManagerViewModel Create();
}