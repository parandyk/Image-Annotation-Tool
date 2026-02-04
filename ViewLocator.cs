using HanumanInstitute.MvvmDialogs.Avalonia;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool;

public class ViewLocator : ViewLocatorBase
{
    protected override string GetViewName(object viewModel)
    {
        return viewModel.GetType().FullName!.Replace("ViewModel", "View");
    }
}