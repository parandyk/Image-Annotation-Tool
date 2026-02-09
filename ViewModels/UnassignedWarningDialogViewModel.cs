using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.ViewModels;

public partial class UnassignedWarningDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public UnassignedWarningDialogViewModel(IStatisticsAggregator statisticsAggregator,
        IClassListProvider classListProvider,
        ImageSpace? image = null)
    {
        DefaultClass = classListProvider.GetDefaultClass();
        
        if (image is not null)
        {
            Global = false;
            AffectedAnnotationCount = image.Annotations.Count(a => a.ClassInfo == DefaultClass);
        }
        else
        {
            Global = true;
            AffectedAnnotationCount = statisticsAggregator.AnnotationsPerClassCounts
                .FirstOrDefault(kvp => kvp.Key == DefaultClass.Id).Value;
        }
    }
    
    [ObservableProperty] 
    private string _title = "Exporting annotations warning";

    private bool Global { get; init; }
    
    public string Description 
    {
        get
        {
            string text;
            
            if (Global)
            {
                text = $"You are about to export annotations across all images. \n" +
                       $"By default, the fallback class \"{DefaultClass.Name}\" is excluded from exporting. \n";
            }
            else
            {
                text = $"You are about to export annotations in the selected image. \n" +
                       $"By default, the fallback class \"{DefaultClass.Name}\" is excluded from exporting. \n";
            }
            
            if (AffectedAnnotationCount == 1)
            {
                text += $"{AffectedAnnotationCount} annotation instance will be excluded. \n"; 
            }
            else if (AffectedAnnotationCount > 1)
            {
                text += $"{AffectedAnnotationCount} annotation instances will be excluded. \n";
            }
            
            return text + "Proceed?. \n";
        }
}

    private ClassData DefaultClass { get; init; } 
    
    public bool IncludeUnassigned { get; set; } = false;

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