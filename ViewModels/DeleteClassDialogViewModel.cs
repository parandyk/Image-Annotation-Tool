using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Kernel;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.ViewModels;

public partial class DeleteClassDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    private readonly IClassListProvider _classListProvider;
    private readonly IStatisticsAggregator _statisticsAggregator;
    
    public DeleteClassDialogViewModel(IClassListProvider classListProvider,
        IStatisticsAggregator statisticsAggregator, 
        ClassData classToDelete, 
        ClassData? substituteClass = null)
    {
        _classListProvider = classListProvider;
        _statisticsAggregator = statisticsAggregator;
        
        ClassToDelete = classToDelete;
        SubstituteClass = substituteClass;
        
        if (substituteClass is null) 
            SubstituteClass = classListProvider.GetDefaultClass();

        AffectedAnnotationCount = statisticsAggregator.AnnotationsPerClassCounts
            .FirstOrDefault(kvp => kvp.Key == classToDelete.Id).Value;
        
        SortedFilteredClassList = new ReadOnlyObservableCollection<ClassData>(_sortedFilteredClassList);
        
        ((INotifyCollectionChanged)_classList).CollectionChanged += OnClassListCollectionChanged;
        
        RebuildClassList();
    }

    private void OnClassListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ClassList));
        RebuildClassList();
    }

    public ClassData ClassToDelete { get; init; } 
    
    public ClassData? SubstituteClass { get; set; } 

    [ObservableProperty] 
    private string _title = "Deleting class";
    
    public string Description
    {
        get
        {

            var text = $"You are about to delete a class called \"{ClassToDelete.Name}\". \n" +
                       $"This operation is reversible. \n";
            
            if (AffectedAnnotationCount == 0)
            {
                return text;
            }
            
            if (AffectedAnnotationCount == 1)
            {
                return text +  
                       $"{AffectedAnnotationCount} annotation instance will be affected. \n" +
                       $"Choose how to proceed.";
            }

            return text +  
                   $"{AffectedAnnotationCount} annotation instances will be affected. \n" +
                   $"Choose how to proceed.";
        }
    }


    private int AffectedAnnotationCount { get;  init; }
    
    
    [ObservableProperty]
    private ClassDeleteButtonOptions? _deleteMode = null;
    
    [ObservableProperty] 
    private ClassSortMode _classSorting = ClassSortMode.None;
    
    [ObservableProperty] 
    private ClassFilterMode _classFiltering = ClassFilterMode.None;

    private ReadOnlyObservableCollection<ClassData> _classList => 
        new ReadOnlyObservableCollection<ClassData>(
            new ObservableCollection<ClassData>(
                _classListProvider.Classes.Where(c => c != ClassToDelete)));
    public ReadOnlyObservableCollection<ClassData> ClassList => _classList;

    private readonly ObservableCollection<ClassData> _sortedFilteredClassList = new();
    public ReadOnlyObservableCollection<ClassData> SortedFilteredClassList { get; }

    public bool? DialogResult { get; set; }

    public event EventHandler? RequestClose;

    partial void OnClassSortingChanged(ClassSortMode oldValue, ClassSortMode newValue)
    {
        RebuildClassList();
    }
    
    partial void OnClassFilteringChanged(ClassFilterMode oldValue, ClassFilterMode newValue)
    {
        RebuildClassList();
    }
    
    [RelayCommand]
    private void Proceed()
    {
        if (DeleteMode is null)
            return;
        
        DialogResult = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
    
    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
    
    private void RebuildClassList()
    {
        List<ClassData>? filteredTempList;
        
        switch (ClassFiltering)
        {
            default:
            case ClassFilterMode.None:
                filteredTempList = _classList.Where(c => c != ClassToDelete).ToList();
                break;
            case ClassFilterMode.HideUnused:
                var unusedGuids = _statisticsAggregator.AnnotationsPerClassCounts
                    .Where(kvp => kvp.Value == 0 && kvp.Key != ClassToDelete.Id).OrderBy(kvp => kvp.Key);
                
                filteredTempList = [];
                
                foreach (var (guid, _) in unusedGuids)
                {
                    filteredTempList.Add(_classListProvider.GetClass(guid));
                }
                break;
            case ClassFilterMode.HideUsed:
                var usedGuids = _statisticsAggregator.AnnotationsPerClassCounts
                    .Where(kvp => kvp.Value > 0 && kvp.Key != ClassToDelete.Id).OrderBy(kvp => kvp.Key);
                
                filteredTempList = [];
                
                foreach (var (guid, _) in usedGuids)
                {
                    filteredTempList.Add(_classListProvider.GetClass(guid));
                }
                break;
        }
        
        List<ClassData>? sortedFilteredTempList;
        
        switch (ClassSorting)
        {
            default:
            case ClassSortMode.None:
                sortedFilteredTempList = filteredTempList.ToList();
                break;
            case ClassSortMode.Oldest:
                sortedFilteredTempList = _classList.IndexOfMany(filteredTempList)
                    .OrderBy(ivm => ivm.Index)
                    .Select(ivm => ivm.Item).ToList();
                break;
            case ClassSortMode.Newest:
                sortedFilteredTempList = _classList.IndexOfMany(filteredTempList)
                    .OrderByDescending(ivm => ivm.Index)
                    .Select(ivm => ivm.Item).ToList();
                break;
            case ClassSortMode.Alphabetical:
                sortedFilteredTempList = filteredTempList.OrderBy(c => c.Name).ToList();
                break;
            case ClassSortMode.ReversedAlphabetical:
                sortedFilteredTempList = filteredTempList.OrderByDescending(c => c.Name).ToList();
                break;
            case ClassSortMode.CountAscending:
                var filteredGuids = filteredTempList.Select(c => c.Id).ToList();
                var ascendingGuids = _statisticsAggregator.AnnotationsPerClassCounts.OrderBy(kvp => kvp.Value)
                    .Where(kvp => filteredGuids.Contains(kvp.Key)).ToList();
                
                sortedFilteredTempList = [];
                foreach (var (guid, _) in ascendingGuids)
                {
                    sortedFilteredTempList.Add(_classListProvider.GetClass(guid));
                }
                break;
            case ClassSortMode.CountDescending:
                filteredGuids = filteredTempList.Select(c => c.Id).ToList();
                var descendingGuids = _statisticsAggregator.AnnotationsPerClassCounts.OrderByDescending(kvp => kvp.Value)
                    .Where(kvp => filteredGuids.Contains(kvp.Key)).ToList();
                
                sortedFilteredTempList = [];
                foreach (var (guid, _) in descendingGuids)
                {
                    sortedFilteredTempList.Add(_classListProvider.GetClass(guid));
                }
                break;
        }

        Dispatcher.UIThread.Post(() =>
        {
            _sortedFilteredClassList.Clear();
            
            foreach (var item in sortedFilteredTempList)
            {
                _sortedFilteredClassList.Add(item);
            }
        });
    }
}
