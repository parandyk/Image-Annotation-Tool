using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IDialogWrapper : IAnnotationDialogWrapper, IClassDialogWrapper, ISettingsDialogWrapper, IImageDialogWrapper
{
    void SetDefaultOwner(INotifyPropertyChanged owner);
    Task<bool?> ShowDialogAsync(IModalDialogViewModel viewModel, INotifyPropertyChanged? ownerViewModel = null);
}
