using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.ViewModels;
using ClassManagerViewModel = ImageAnnotationTool.ViewModels.ClassManagerViewModel;
using MainViewModel = ImageAnnotationTool.ViewModels.MainViewModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IClassManagerViewModelFactory
{
    public ClassManagerViewModel Create();
}