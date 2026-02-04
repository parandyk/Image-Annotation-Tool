using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.ViewModels;

public partial class DeleteAnnotationDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public DeleteAnnotationDialogViewModel(Annotation annotationToDelete)
    {
        AnnotationToDelete = annotationToDelete;
    }

    [ObservableProperty] 
    private string _title = "Deleting annotation";
    
    public string Description => $"You are about to delete annotation \"" +
                                 string.Format(AnnotationNameFormat.AnnotationNameFormatString, 
                                     AnnotationToDelete.DisplayId,
                                     AnnotationToDelete.ClassInfo.Name) + $"\".\n" + 
                                 "This operation is reversible. \n";
    
    [RelayCommand]
    private void Proceed()
    {
        DialogResult = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public bool DontAskAgain { get; set; } = false;
    
    private Annotation AnnotationToDelete { get; init; }

    public bool? DialogResult { get; set; }

    public event EventHandler? RequestClose;
}