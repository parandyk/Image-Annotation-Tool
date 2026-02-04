using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.ViewModels;

public partial class RenameClassDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    private readonly IClassListProvider _classListProvider;
    private readonly IClassDataPolicy _classDataPolicy;
    
    public RenameClassDialogViewModel(
        IClassListProvider classListProvider, 
        IStatisticsAggregator statisticsAggregator,
        IClassDataPolicy classDataPolicy,
        ClassData classToRename)
    {
        _classListProvider = classListProvider;
        _classDataPolicy = classDataPolicy;
        ClassToRename = classToRename;
        
        AffectedAnnotationCount = statisticsAggregator.AnnotationsPerClassCounts
            .FirstOrDefault(kvp => kvp.Key == classToRename.Id).Value;
    }

    public bool NameAvailable => ValidateName();
    
    public ClassData ClassToRename { get; init; } 
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NameAvailable))]
    private string _newClassName = string.Empty;

    [ObservableProperty] 
    private string _title = "Renaming class";
    
    public string Description
    {
        get
        {
            var text = $"You are about to rename a class called \"{ClassToRename.Name}\". \n" +
                       $"This operation is reversible. \n";
            
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

    private int AffectedAnnotationCount { get;  init; }
    
    public bool? DialogResult { get; private set; }
    public event EventHandler? RequestClose;
    
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

    private bool ValidateName()
    {
        return _classDataPolicy.IsNameValid(NewClassName) && 
               Equals(ClassToRename.Name, NewClassName) && 
               _classListProvider.ClassExists(ClassToRename.Id);
    }
}