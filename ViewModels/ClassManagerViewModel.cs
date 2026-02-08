using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Kernel;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.Infrastructure.SettingsStore;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Factories;
using ImageAnnotationTool.Interfaces;

namespace ImageAnnotationTool.ViewModels;

public partial class ClassManagerViewModel : ObservableObject
{
    private readonly IClassStore  _classStore;
    private readonly INotificationSettings _notificationSettings;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly IAppMessenger _messenger;
    private readonly IStatisticsAggregator _statisticsAggregator;

    public ClassManagerViewModel(
        IClassStore classStore,
        IStatisticsAggregator statisticsAggregator,
        INotificationSettings notificationSettings,
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger)
    {
        _messenger = messenger;
        _notificationSettings = notificationSettings;
        _dialogWrapper = dialogWrapper;
        
        _classStore = classStore;
        _classStore.PropertyChanged += OnClassStoreChanged;
        
        _statisticsAggregator = statisticsAggregator;
        
        _activeClass = _classStore.ActiveClass;
        _defaultClass = _classStore.GetDefaultClass();
        
        SortedFilteredClassList = new ReadOnlyObservableCollection<ClassData>(_sortedFilteredClassList);
        
        RebuildClassList();
        
        ((INotifyCollectionChanged)_classList).CollectionChanged += OnClassListCollectionChanged;
    }
    
    partial void OnClassSortingChanged(ClassSortMode oldValue, ClassSortMode newValue)
    {
        RebuildClassList();
    }
    
    partial void OnClassFilteringChanged(ClassFilterMode oldValue, ClassFilterMode newValue)
    {
        RebuildClassList();
    }

    private void OnClassStoreChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_classStore.ActiveClass))
        {
            OnPropertyChanged(nameof(ActiveClass));
        }
    }

    private void OnClassListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null || e.NewItems != null)
        {
            if (e.NewItems != null)
            {
                foreach (ClassData item in e.NewItems)
                {
                    item.PropertyChanged += OnClassPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ClassData item in e.OldItems)
                {
                    item.PropertyChanged -= OnClassPropertyChanged;
                }
            }
            
            OnPropertyChanged(nameof(ClassList));
            RebuildClassList();
        }
    }

    private void OnClassPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RebuildClassList();
    }
    
    private ReadOnlyObservableCollection<ClassData> _classList => _classStore.Classes;
    public ReadOnlyObservableCollection<ClassData> ClassList => _classList;

    private readonly ObservableCollection<ClassData> _sortedFilteredClassList = new();
    public ReadOnlyObservableCollection<ClassData> SortedFilteredClassList { get; }

    private ClassData _activeClass;

    public ClassData ActiveClass
    {
        get => _activeClass;
        set
        {
            if (value is null)
                return;
            
            if (_classStore.ActiveClass == value) return;
            _classStore.ActiveClass = value;
            
            SetProperty(ref _activeClass, value);
            OnPropertyChanged();
        }
    }

    private readonly ClassData _defaultClass;

    public ClassData DefaultClass
    {
        get => _defaultClass;
    }

    [ObservableProperty] 
    private string _inputName = string.Empty;
    
    [ObservableProperty] 
    private ClassSortMode _classSorting = ClassSortMode.None;
    
    [ObservableProperty] 
    private ClassFilterMode _classFiltering = ClassFilterMode.None;

    private void RebuildClassList()
    {
        List<ClassData>? filteredTempList;
        
        switch (ClassFiltering)
        {
            default:
            case ClassFilterMode.None:
                filteredTempList = _classList.ToList();
                break;
            case ClassFilterMode.HideUsed:
                var unusedGuids = _statisticsAggregator.AnnotationsPerClassCounts
                    .Where(kvp => kvp.Value == 0).OrderBy(kvp => kvp.Key);
                
                filteredTempList = [];
                
                foreach (var (guid, _) in unusedGuids)
                {
                    filteredTempList.Add(_classStore.GetClass(guid));
                }
                break;
            case ClassFilterMode.HideUnused:
                var usedGuids = _statisticsAggregator.AnnotationsPerClassCounts
                    .Where(kvp => kvp.Value > 0).OrderBy(kvp => kvp.Key);
                
                filteredTempList = [];
                
                foreach (var (guid, _) in usedGuids)
                {
                    filteredTempList.Add(_classStore.GetClass(guid));
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
                    sortedFilteredTempList.Add(_classStore.GetClass(guid));
                }
                break;
            case ClassSortMode.CountDescending:
                filteredGuids = filteredTempList.Select(c => c.Id).ToList();
                var descendingGuids = _statisticsAggregator.AnnotationsPerClassCounts.OrderByDescending(kvp => kvp.Value)
                    .Where(kvp => filteredGuids.Contains(kvp.Key)).ToList();
                
                sortedFilteredTempList = [];
                foreach (var (guid, _) in descendingGuids)
                {
                    sortedFilteredTempList.Add(_classStore.GetClass(guid));
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
    
    [RelayCommand]
    private async Task ChangeClassName(Guid guid)
    {
        try
        {
            var classData = _classStore.GetClass(guid);
            
            var vm = _dialogWrapper.CreateRenameClassDialog(classData);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                _classStore.RenameClass(guid, vm.NewClassName);
                _messenger.SendErrorOccurredNotification("Class renamed successfully.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Class renaming cancelled.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void ChangeClassVisibility(Guid guid)
    {
        try
        {
            var classData = _classStore.GetClass(guid);
            _classStore.ChangeVisibilityClass(guid, !classData.IsVisible);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }

    [RelayCommand]
    private void CreateNewClass(string? inputName)
    {
        try
        {
            if (!string.IsNullOrEmpty(inputName) && inputName.Length > 0)
            {
                _classStore.AddClass(inputName);
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }

    [RelayCommand]
    private async Task DeleteClass(Guid guid)
    {
        try
        {
            if (_statisticsAggregator.AnnotationsPerClassCounts.FirstOrDefault(kvp => kvp.Key == guid).Value == 0)
            {
                _classStore.RemoveClass(guid);
                _messenger.SendErrorOccurredNotification("Class deleted successfully.");
                return;
            }
            
            ClassData substitute = _classStore.GetDefaultClass();
            ClassData forDeletion = _classStore.GetClass(guid); 
            
            var vm = _dialogWrapper.CreateDeleteClassDialog(forDeletion, substitute);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                switch (vm.DeleteMode) //TODO
                {
                    case ClassDeleteButtonOptions.SwapToSpecific:
                        substitute = vm.SubstituteClass!;
                        break;
                    case ClassDeleteButtonOptions.SwapIndividually:
                        //TODO
                        break;
                    case ClassDeleteButtonOptions.SwapToUnassigned:
                    case ClassDeleteButtonOptions.DeleteAffected:
                        break;
                    default:
                        _messenger.SendErrorOccurredNotification("Cannot delete class.");
                        return;
                }

                if (ActiveClass == forDeletion)
                {
                    ActiveClass = substitute;
                }
                
                if (vm.DeleteMode is ClassDeleteButtonOptions.DeleteAffected)
                {
                    _classStore.RemoveClass(guid);
                }
                else if (vm.DeleteMode is ClassDeleteButtonOptions.SwapToSpecific)
                {
                    _classStore.RemoveClass(guid, substitute.Id);
                }
                else if (vm.DeleteMode is ClassDeleteButtonOptions.SwapToUnassigned)
                {
                    _classStore.RemoveClass(guid, _classStore.GetDefaultClass().Id);
                }
                else if (vm.DeleteMode is ClassDeleteButtonOptions.SwapIndividually)
                {
                    //TODO
                }
               
                _messenger.SendErrorOccurredNotification("Class deleted successfully.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Class deletion cancelled.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
}