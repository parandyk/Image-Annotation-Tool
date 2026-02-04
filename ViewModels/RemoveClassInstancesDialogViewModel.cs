using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.ViewModels;

public partial class RemoveClassInstancesDialogViewModel  : ObservableObject, IModalDialogViewModel, ICloseable
{
    public RemoveClassInstancesDialogViewModel(
        IStatisticsAggregator statisticsAggregator,
        ClassData classForRemoval,
        ImageSpace? image = null)
    {
        ClassForRemoval = classForRemoval;

        if (image is not null)
        {
            Global = false;
            AffectedAnnotationCount = image.Annotations.Count(a => a.ClassInfo == classForRemoval);
        }
        else
        {
            Global = true;
            AffectedAnnotationCount = statisticsAggregator.AnnotationsPerClassCounts
                .FirstOrDefault(kvp => kvp.Key == classForRemoval.Id).Value;
        }
    }
    
    [ObservableProperty] 
    private string _title = "Deleting class instances";

    private bool Global { get; init; }
    
    public string Description 
    {
        get
        {
            string text;
            
            if (Global)
            {
                text = $"You are about to delete instances of class \"{ClassForRemoval.Name}\" across all images. \n" +
                       $"This operation is reversible. \n";
            }
            else
            {
                text = $"You are about to delete instances of class \"{ClassForRemoval.Name}\" present in this image. \n" +
                       $"This operation is reversible. \n";
            }
            
            if (AffectedAnnotationCount == 0)
            {
                return text;
            }
            
            if (AffectedAnnotationCount == 1)
            {
                return text + $"{AffectedAnnotationCount} annotation instance will be affected."; 
            }
            
            return text + $"{AffectedAnnotationCount} annotation instances will be affected."; 
        }
}

    private ClassData ClassForRemoval { get; init; } 
    
    public bool DontAskAgain { get; set; } = false;
    
    private int AffectedAnnotationCount { get; init; }
    
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
    
    public bool? DialogResult { get; set; }
    public event EventHandler? RequestClose;
}