using ImageAnnotationTool.Domain.Entities;
using ImageViewModel = ImageAnnotationTool.ViewModels.ImageViewModel;

namespace ImageAnnotationTool.Factories;

public interface IImageViewModelFactory
{
    ImageViewModel Create(ImageSpace imageSpace);
}