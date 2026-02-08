using System.ComponentModel;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs;

namespace ImageAnnotationTool.Factories;

public interface IDialogWrapper : IAnnotationDialogWrapper, IClassDialogWrapper, ISettingsDialogWrapper, IImageDialogWrapper
{
    void SetDefaultOwner(INotifyPropertyChanged owner);
    Task<bool?> ShowDialogAsync(IModalDialogViewModel viewModel, INotifyPropertyChanged? ownerViewModel = null);
}
