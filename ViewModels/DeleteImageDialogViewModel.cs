using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.ViewModels;

public partial class DeleteImageDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public DeleteImageDialogViewModel(ImageSpace imageToDelete)
    {
        ImageToDelete = imageToDelete;
    }

    [ObservableProperty] 
    private string _title = "Deleting image";

    public string Description
    {
        get
        {
            var text = $"You are about to delete image \"{ImageToDelete.Source.ImageName}\". \n" +
                       $"This operation is reversible. \n";
            
            if (ImageToDelete.Annotations.Count == 0)
            {
                return text;
            }
            
            if (ImageToDelete.Annotations.Count == 1)
            {
                return text +  
                       $"There is \"{ImageToDelete.Annotations.Count}\" " +
                       $"annotation present in this image.";
            }
            
            return text +  
                   $"There is a total of \"{ImageToDelete.Annotations.Count}\" " +
                   $"annotations present in this image.";
        }
    }


    public bool DontAskAgain { get; set; } = false;
    
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
    
    private ImageSpace ImageToDelete { get; init; } 
    
    public bool? DialogResult { get; set; }
    public event EventHandler? RequestClose;
}