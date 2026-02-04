using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.ViewModels;
using ImageManagerViewModel = ImageAnnotationTool.ViewModels.ImageManagerViewModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IImageManagerViewModelFactory
{
    public ImageManagerViewModel Create();
}