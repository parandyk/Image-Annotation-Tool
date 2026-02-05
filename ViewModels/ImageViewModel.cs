using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AtomUI.Desktop.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Models;
using IAnnotationViewModelFactory = ImageAnnotationTool.Domain.Infrastructure.IAnnotationViewModelFactory;
using IClassListProvider = ImageAnnotationTool.Domain.Infrastructure.IClassListProvider;


namespace ImageAnnotationTool.ViewModels;

public partial class ImageViewModel : ObservableObject, IDisposable
{
    private readonly IClassListProvider _classListProvider;
    private readonly INotificationSettings _notificationSettings;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly ICommandStack _undoRedoService;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IAnnotationViewModelFactory _annotationViewModelFactory;
    private readonly IAppModeSettingsProvider _annotationModes;
    private readonly IAppMessenger _messenger;
    private readonly ImageSpace _imageSpace;
    
    public ImageViewModel(
        IClassListProvider classListProvider,
        INotificationSettings notificationSettings,
        IDialogWrapper dialogWrapper,
        IAnnotationViewModelFactory annotationViewModelFactory,
        ICommandStack undoRedoService,
        IWorkspaceCommandFactory commandFactory,
        IAppModeSettingsProvider annotationModes,
        IAppMessenger messenger,
        ImageSpace imageSpace)
    {
        _imageSpace = imageSpace;
        
        _annotationViewModelFactory  = annotationViewModelFactory;
        _undoRedoService = undoRedoService;
        _commandFactory = commandFactory;
        
        _classListProvider = classListProvider;
        _notificationSettings = notificationSettings;
        _dialogWrapper = dialogWrapper;
        
        _annotationModes = annotationModes;
        _annotationModes.PropertyChanged += OnAnnotationModeChanged;

        _messenger = messenger;
        
        ((INotifyCollectionChanged)_imageSpace.Annotations).CollectionChanged += OnDomainChanged;
        _annotationList.CollectionChanged += OnAnnotationListChanged;
        _annotationList.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(BBoxesPresentLocal));
        };
        
        SortedFilteredAnnotationList = new ReadOnlyObservableCollection<AnnotationViewModel>(_sortedFilteredAnnotationList);
        ((INotifyCollectionChanged)_classList).CollectionChanged += OnClassListCollectionChanged;
        
        _messenger.Register<AbortOperationMessage>(this, OnAbortRequested);
        
        if (_imageSpace.Annotations.Any()) 
            DrawAnnotations();
        
        RebuildAnnotationDisplayList();
    }

    private void OnClassListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    { 
        OnPropertyChanged(nameof(ClassList));
    }
    
    private ReadOnlyObservableCollection<ClassData> _classList => _classListProvider.Classes;
    public ReadOnlyObservableCollection<ClassData> ClassList => _classList;
    
    private void DrawAnnotations()
    {
        foreach (var annotation in _imageSpace.Annotations)
        {
            if (_annotationList.Any(avm => avm.AnnotationData == annotation))
                continue;
            
            var avm = CreateAnnotationViewModel(annotation, false);
            _annotationList.Add(avm);
        }
        
        SelectedAnnotation = _annotationList.Last();
        SelectedAnnotation.IsLastDrawn = true;
    }
    
    private void OnDomainChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (Annotation annotation in e.OldItems)
            {
                RemoveAnnotationViewModel(annotation);
            }

            var fallback = _annotationList.LastOrDefault();

            if (fallback != null)
            {
                SelectedAnnotation = fallback;
                SelectedAnnotation.IsLastDrawn = true;
            }
        }

        if (e.NewItems != null)
        {
            foreach (Annotation annotation in e.NewItems)
            {
                if (_annotationList.Any(avm => avm.AnnotationData == annotation))
                    continue;
                
                var avm = CreateAnnotationViewModel(annotation, false);
                _annotationList.Add(avm);
            }

            SelectedAnnotation = _annotationList.Last();
            SelectedAnnotation.IsLastDrawn = true;
        }
    }
    
    public ReadOnlyObservableCollection<AnnotationViewModel> AnnotationList => 
        new ReadOnlyObservableCollection<AnnotationViewModel>(_annotationList);
    
    private readonly ObservableCollection<AnnotationViewModel> _annotationList = [];
    
    private readonly ObservableCollection<AnnotationViewModel> _sortedFilteredAnnotationList = new();
    
    public ReadOnlyObservableCollection<AnnotationViewModel> SortedFilteredAnnotationList { get; }

    public ObservableCollection<Guid> LastClickBBoxCycleList
    {
        get;
    } = [];
    
    public ImageSpace ImageSpace => _imageSpace;
    
    public string ImageName => _imageSpace.Source.ImageName;
    
    //doubles
    
    [ObservableProperty]
    private double _pointerPX;
    
    [ObservableProperty] 
    private double _pointerPY;

    [ObservableProperty] 
    private double? _valueX1NumericBox; 
    
    [ObservableProperty] 
    private double? _valueX2NumericBox; 
    
    [ObservableProperty] 
    private double? _valueY1NumericBox; 
    
    [ObservableProperty] 
    private double? _valueY2NumericBox;
    
    [ObservableProperty] 
    private double? _valueWidthNumericBox; 
    
    [ObservableProperty] 
    private double? _valueHeightNumericBox;
    
    //ints
    public Guid GUID { get; } = Guid.NewGuid();

    private int _cycleIndex = 0;
    
    //bools

    [ObservableProperty] 
    private bool _isSelected;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddPointCommand))]
    private bool _pointerValid = false;
    
    [ObservableProperty]
    private bool _pointerOverWorkspace;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PointerMovedRedrawBBoxCommand))]
    [NotifyCanExecuteChangedFor(nameof(RedrawBBoxCommand))]
    private bool _annotatingOn = false;
    
    public bool CanMoveForwardAnnotation => SelectedAnnotation is not null && 
                                            _sortedFilteredAnnotationList.Count > 1 && 
                                            _sortedFilteredAnnotationList.IndexOf(SelectedAnnotation) < 
                                            _sortedFilteredAnnotationList.IndexOf(_sortedFilteredAnnotationList.Last());
    public bool CanMoveBackwardAnnotation => SelectedAnnotation is not null &&
                                             _sortedFilteredAnnotationList.Count > 1 && 
                                             _sortedFilteredAnnotationList.IndexOf(SelectedAnnotation) > 
                                             _sortedFilteredAnnotationList.IndexOf(_sortedFilteredAnnotationList.First());
    public bool BBoxesPresentLocal => _annotationList.Count > 0;
    public bool EditingModeOn => _annotationModes.EditingModeOn;
    private bool CanAddPoint => !_annotationModes.EditingModeOn;
    private bool CanRedraw => AnnotatingOn && !_annotationModes.EditingModeOn;
    
    //custom variables
    public AnnotationAddingMode AddingMode => _annotationModes.AddingMode;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanMoveForwardAnnotation))]
    [NotifyPropertyChangedFor(nameof(CanMoveBackwardAnnotation))]
    private AnnotationViewModel? _selectedAnnotation;
    
    [RelayCommand]
    private async Task SwapClassInstances(Guid classGuid)
    {
        if (_annotationList.Count(avm => avm.AnnotationData.ClassInfo.Id == classGuid) == 0)
        {
            _messenger.SendErrorOccurredNotification("No instances were found.");
            return;
        }
        
        try
        {
            var classData = _classListProvider.GetClass(classGuid);

            var vm = _dialogWrapper.CreateSwapClassInstancesDialog(classData);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                var newClass = vm.SubstituteClass;
                var annotations = _annotationList.Where(avm => avm.AnnotationData.ClassInfo.Id == classGuid).Select(avm => avm.AnnotationData).ToList();
                
                _undoRedoService.Execute(_commandFactory.ChangeClassBatchAnnotation(annotations, newClass));
                
                _messenger.SendErrorOccurredNotification("Class instances swapped successfully.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Class instances swap cancelled.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private async Task RemoveClassInstances(Guid classGuid)
    {
        if (_annotationList.Count(avm => avm.AnnotationData.ClassInfo.Id == classGuid) == 0)
        {
            _messenger.SendErrorOccurredNotification("No instances were found.");
            return;
        }

        var annotations = _annotationList.Where(avm =>
                avm.AnnotationData.ClassInfo.Id == classGuid)
            .Select(avm => avm.AnnotationData).ToList();
        
        if (_notificationSettings.SuppressLocalClassInstanceDeletionDialogs)
        {
            _undoRedoService.Execute(new RemoveBatchAnnotationCommand(ImageSpace, annotations));
            _messenger.SendErrorOccurredNotification("Class instances deleted successfully.");
            return;
        }
        
        try
        {
            var classData = _classListProvider.GetClass(classGuid);

            var vm = _dialogWrapper.CreateRemoveClassInstancesDialog(classData);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {

                if (vm.DontAskAgain)
                    _notificationSettings.ChangeSuppressionLocalClassInstanceDeletionDialogs(true);
                
                _undoRedoService.Execute(new RemoveBatchAnnotationCommand(ImageSpace, annotations));
                
                _messenger.SendErrorOccurredNotification("Class instances deleted successfully.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Class instances deletion cancelled.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private async Task ChangeAnnotationClass(Guid annotationGuid)
    {
        try
        {
            var annotation = _annotationList.FirstOrDefault(avm => avm.GUID == annotationGuid);

            if (annotation is null)
            {
                _messenger.SendErrorOccurredNotification("Annotation not found.");
                return;
            }
            var annotationData = annotation.AnnotationData;
            var vm = _dialogWrapper.CreateChangeAnnotationClassDialog(annotationData);
            
            var result = await _dialogWrapper.ShowDialogAsync(vm); // may require passing main vm 

            if (result is true)
            {
                _undoRedoService.Execute(_commandFactory.ChangeClassAnnotation(annotationData, vm.SubstituteClass));
                _messenger.SendErrorOccurredNotification("Annotation class changed successfully.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Annotation class change aborted.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private async Task DeleteAnnotation(Guid annotationGuid)
    {
        try
        {
            var annotation = _annotationList.FirstOrDefault(avm => avm.GUID == annotationGuid);

            if (annotation is null)
            {
                _messenger.SendErrorOccurredNotification("Annotation not found.");
                return;
            }
            
            var annotationData = annotation.AnnotationData;
            
            if (!_imageSpace.AnnotationExists(annotationData))
            {
                _messenger.SendErrorOccurredNotification("Annotation not found.");
                return;
            }
            
            if (_notificationSettings.SuppressAnnotationDeletionDialogs)
            {
                _undoRedoService.Execute(new RemoveAnnotationCommand(ImageSpace, annotationData));
                _messenger.SendErrorOccurredNotification("Annotation deleted successfully.");
                return;
            }
        
            var vm = _dialogWrapper.CreateDeleteAnnotationDialog(annotationData);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                if (vm.DontAskAgain)
                    _notificationSettings.ChangeSuppressionAnnotationDeletionDialogs(true);
                    
                _undoRedoService.Execute(new RemoveAnnotationCommand(ImageSpace, annotationData));
                _messenger.SendErrorOccurredNotification("Annotation deleted successfully.");
                return;
            }
                
            _messenger.SendErrorOccurredNotification("Annotation deletion cancelled.");
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    private void OnAbortRequested(object recipient, AbortOperationMessage message)
    {
        AbortDrawing();
    }

    [RelayCommand]
    private void CompareCycleLists(List<Guid>? newCycleList)
    {
        if (newCycleList is null)
        {
            ChangeAnnotation(null);
            return;
        }
        
        if (newCycleList.Count == 0)
        {
            LastClickBBoxCycleList.Clear();
            _cycleIndex = 0;
            ChangeAnnotation(null);
            return;
        }
        
        if (newCycleList.Count != LastClickBBoxCycleList.Count
            || !newCycleList.SequenceEqual(LastClickBBoxCycleList))
        {
            LastClickBBoxCycleList.Clear();
            _cycleIndex = 0;

            foreach (var value in newCycleList)
            {
                LastClickBBoxCycleList.Add(value);
            }
        }

        bool selectionPresent = SelectedAnnotation is not null; 
        
        CycleList(selectionPresent);
    }
    
    private void CycleList(bool selectionPresent)
    {
        if (!selectionPresent)
        {
            var newGUID = LastClickBBoxCycleList.ElementAt(_cycleIndex);
            ChangeAnnotation(newGUID);
            return;
        }
            
        while (true)
        {
            if (_cycleIndex >= LastClickBBoxCycleList.Count - 1)
            {
                _cycleIndex = 0;
            }
            else
            {
                _cycleIndex++;
            }

            var newGUID = LastClickBBoxCycleList.ElementAt(_cycleIndex);
            
            if (newGUID == SelectedAnnotation?.GUID && LastClickBBoxCycleList.Count > 1) continue;
            
            ChangeAnnotation(newGUID);
            break;
        }
    }
    
    partial void OnAnnotationSortingChanged(AnnotationSortMode oldValue, AnnotationSortMode newValue)
    {
        RebuildAnnotationDisplayList();
    }
    
    partial void OnAnnotationFilteringChanged(AnnotationFilterMode oldValue, AnnotationFilterMode newValue)
    {
        RebuildAnnotationDisplayList();
    }
    
    private void OnAnnotationListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (AnnotationViewModel item in e.NewItems)
            {
                item.PropertyChanged += OnAnnotationPropertyChanged;
                RebuildAnnotationDisplayList();
            }
        }
        
        if (e.OldItems != null)
        {
            foreach (AnnotationViewModel item in e.OldItems)
            {
                item.PropertyChanged -= OnAnnotationPropertyChanged;
                RebuildAnnotationDisplayList();
            }
        }
    }

    private void RebuildAnnotationDisplayList()
    {
        List<AnnotationViewModel>? filteredTempList;

        switch (AnnotationFiltering)
        {
            default:
            case AnnotationFilterMode.None:
                filteredTempList = _annotationList.ToList();
                break;
            case AnnotationFilterMode.HideAssigned:
                filteredTempList = _annotationList.Where(avm => avm.AnnotationData.ClassInfo == _classListProvider.GetDefaultClass()).ToList();
                break;
            case AnnotationFilterMode.HideUnassigned:
                filteredTempList = _annotationList.Where(avm => avm.AnnotationData.ClassInfo != _classListProvider.GetDefaultClass()).ToList();
                break;
        }
        
        List<AnnotationViewModel>? sortedFilteredTempList;
        
        switch (AnnotationSorting)
        { 
            default:
            case AnnotationSortMode.None:
                sortedFilteredTempList = filteredTempList.ToList();
                break;
            case AnnotationSortMode.Oldest:
                sortedFilteredTempList = filteredTempList.OrderBy(avm => avm.GUID).ToList();
                break;
            case AnnotationSortMode.Newest:
                sortedFilteredTempList = filteredTempList.OrderByDescending(avm => avm.GUID).ToList();
                break;
            case AnnotationSortMode.Alphabetical:
                sortedFilteredTempList = filteredTempList.OrderBy(avm => avm.AssignedClass.Name).ToList();
                break;
            case AnnotationSortMode.ReversedAlphabetical:
                sortedFilteredTempList = filteredTempList.OrderByDescending(avm => avm.AssignedClass.Name).ToList();
                break;
            case AnnotationSortMode.LargestFirst:
                sortedFilteredTempList = filteredTempList.OrderByDescending(avm => avm.Width * avm.Height).ToList();
                break;
            case AnnotationSortMode.SmallestFirst:
                sortedFilteredTempList = filteredTempList.OrderBy(avm => avm.Width * avm.Height).ToList();
                break;
            case AnnotationSortMode.CountAscending:
                sortedFilteredTempList = filteredTempList
                    .GroupBy(avm => avm.AssignedClass.Name)
                    .OrderBy(g => g.Count())
                    .SelectMany(g => g).ToList();
                break;
            case AnnotationSortMode.CountDescending:
                sortedFilteredTempList = filteredTempList
                    .GroupBy(avm => avm.AssignedClass.Name)
                    .OrderByDescending(g => g.Count())
                    .SelectMany(g => g).ToList();
                break;
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            _sortedFilteredAnnotationList.Clear();
            
            foreach (var item in sortedFilteredTempList)
            {
                _sortedFilteredAnnotationList.Add(item);
            }
        });
    }
    
    [ObservableProperty] 
    private AnnotationSortMode _annotationSorting = AnnotationSortMode.None;
    
    [ObservableProperty] 
    private AnnotationFilterMode _annotationFiltering = AnnotationFilterMode.None;
    
    partial void OnSelectedAnnotationChanged(AnnotationViewModel? oldValue, AnnotationViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.IsSelected = false;
        }
        
        if (newValue != null)
        {
            newValue.IsSelected = true;
        }
    }
    
    // used for changing bbox ID when the class changes for that bbox, to ensure proper numbering
    private void OnAnnotationPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (sender is AnnotationViewModel avm)
            {
                if (e.PropertyName == nameof(AnnotationViewModel.PreviewingEnabled))
                {
                    if (!avm.PreviewingEnabled)
                    {
                        var bbox = new BoundingBox(
                            avm.X,
                            avm.Y,
                            avm.X + avm.Width,
                            avm.Y + avm.Height);
                        try
                        {
                            _undoRedoService.Execute(new MoveAnnotationCommand(ImageSpace, avm.AnnotationData, bbox));
                        }
                        catch (Exception exception)
                        {
                            _messenger.SendErrorOccurredNotification(exception.Message);
                            avm.CancelPositionMutationCommand.Execute(null);
                        }
                    }
                }

                if (e.PropertyName == nameof(AnnotationViewModel.AssignedClass))
                {
                    RebuildAnnotationDisplayList();
                }
            }
        }
        catch (Exception exception)
        {
            _messenger.SendErrorOccurredNotification(exception.Message);
        }

    }

    private void ValidatePointerValues(ValueTuple<double, double>? pointerTuple)
    {
        if (pointerTuple == null)
        {
            PointerValid = false;
            return;
        }

        PointerPX = pointerTuple.Value.Item1;
        PointerPY =  pointerTuple.Value.Item2;
        
        PointerValid =  true;
    }
    
    [RelayCommand(CanExecute = nameof(CanAddPoint))]
    private void AddPoint(ValueTuple<double, double>? pointerTuple)
    {
        if (!PointerOverWorkspace && !AnnotatingOn) return;
        
        ValidatePointerValues(pointerTuple);

        if (!PointerValid)
        {
            // _messenger.SendErrorOccurredNotification("Invalid input. Use LMB for drawing/moving.");
            return;
        }
        
        try
        {
            var pointerPosition = GetClampedPointer(PointerPX, PointerPY);
            
            if (!AnnotatingOn)
            {
                StartAnnotation(pointerPosition);
            }
            else
            {
                if (!FinishAnnotation(pointerPosition))
                {
                    if (AddingMode is AnnotationAddingMode.DragDraw)
                        AbortDrawing();
                    
                    return;
                }

                if (SelectedAnnotation!.Temporary)
                {
                    var bbox = new BoundingBox(
                        SelectedAnnotation.X, 
                        SelectedAnnotation.Y, 
                        SelectedAnnotation.X + SelectedAnnotation.Width, 
                        SelectedAnnotation.Y + SelectedAnnotation.Height);
                    
                    _annotationList.Remove(SelectedAnnotation);
                    SelectedAnnotation.DeleteAnnotation();
                    
                    AddAnnotation(bbox);
                }
            }
        
            AnnotatingOn = !AnnotatingOn;
        }
        catch (Exception excpt)
        {
            _messenger.SendErrorOccurredNotification(excpt.Message);
        }
    }

    [RelayCommand]
    private void ChangeAnnotationVisibility(Guid annotationGuid)
    {
        try
        {
            var annotation = _annotationList.FirstOrDefault(avm => avm.GUID == annotationGuid);

            if (annotation is null)
            {
                _messenger.SendErrorOccurredNotification("Annotation not found.");
                return;
            }
            
            var annotationData = annotation.AnnotationData;
            _undoRedoService.Execute(new ChangeVisibilityAnnotationCommand(annotationData, null));
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    private void AddAnnotation(BoundingBox bbox)
    {
        try
        {
            _undoRedoService.Execute(new AddAnnotationCommand(ImageSpace, bbox, _classListProvider.GetActiveClass()));
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    partial void OnAnnotatingOnChanged(bool oldValue, bool newValue)
    {
        OnPropertyChanged(nameof(CanRedraw));
    }
    
    private void OnAnnotationModeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_annotationModes.EditingModeOn))
        {
            OnPropertyChanged(nameof(EditingModeOn));
            OnPropertyChanged(nameof(CanRedraw));
            OnPropertyChanged(nameof(CanAddPoint));
            
            if (AnnotatingOn)
                AbortDrawing();
        }
        
        else if (e.PropertyName == nameof(_annotationModes.AddingMode))
        {
            OnPropertyChanged(nameof(AddingMode));
        }
    }
    
    [RelayCommand]
    private void AbortDrawing()
    {
        if (!AnnotatingOn)
        {
            if (SelectedAnnotation is not null)
            {
                if (!SelectedAnnotation.PreviewingEnabled)
                {
                    SelectedAnnotation.IsSelected = !_annotationModes.EditingModeOn && SelectedAnnotation.IsSelected;
                }
            }
            
            return;
        }

        RemoveTemporaryAnnotation();
    }

    private void RemoveTemporaryAnnotation()
    {
        if (SelectedAnnotation != null && SelectedAnnotation.Temporary)
        {
            _annotationList.Remove(SelectedAnnotation);
            SelectedAnnotation.DeleteAnnotation();
            
            if (_annotationList.Any())
            {
                SelectedAnnotation = _annotationList.Last();
            }
            else
            {
                SelectedAnnotation = null;
            }
            
            AnnotatingOn = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRedraw))]
    private void PointerMovedRedrawBBox(ValueTuple<double, double>? pointerTuple)
    {
        ValidatePointerValues(pointerTuple);

        if (!PointerValid) 
            return;
        
        RedrawBBox();
    }
    
    [RelayCommand(CanExecute = nameof(AnnotatingOn))]
    private void RedrawBBox()
    {
        var clampedPointer = GetClampedPointer(PointerPX, PointerPY);

        RedrawAnnotation(clampedPointer);
    }

    private PointerPositionModel GetClampedPointer(PointerPositionModel pointerPosition)
    {
        return GetClampedPointer(pointerPosition.PX, pointerPosition.PY);
    }

    private PointerPositionModel GetClampedPointer(double pointerX, double pointerY)
    {
        var clampedX = Math.Clamp(pointerX, 0, _imageSpace.Metadata.ImagePixelWidth);
        var clampedY = Math.Clamp(pointerY, 0, _imageSpace.Metadata.ImagePixelHeight);
        
        return new PointerPositionModel { PX = clampedX, PY = clampedY };
    }
    
    [RelayCommand]
    private void ChangeAnnotationAnchoring(Guid annotationGuid)
    {
        try
        {
            var annotation = _annotationList.FirstOrDefault(avm => avm.GUID == annotationGuid);

            if (annotation is null)
            {
                _messenger.SendErrorOccurredNotification("Annotation not found.");
                return;
            }
            
            var annotationData = annotation.AnnotationData;
            
            _undoRedoService.Execute(new ChangeAnchoringAnnotationCommand(annotationData, null));
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void MoveAnnotationForward()
    {
        try
        {
            if (_sortedFilteredAnnotationList.Count <= 0) return;
        
            if (SelectedAnnotation is null) return;
        
            var newIdx = _sortedFilteredAnnotationList.IndexOf(SelectedAnnotation);
            var newAnnotation = _sortedFilteredAnnotationList.ElementAtOrDefault(newIdx + 1);
            
            if (newAnnotation is null)
            {
                newAnnotation = _sortedFilteredAnnotationList.First();
            }
        
            ChangeAnnotation(newAnnotation.GUID);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void MoveAnnotationBackward()
    {
        try
        {
            if (_sortedFilteredAnnotationList.Count <= 0) return;
        
            if (SelectedAnnotation is null) return;
            
            var newIdx = _sortedFilteredAnnotationList.IndexOf(SelectedAnnotation);
            var newAnnotation = _sortedFilteredAnnotationList.ElementAtOrDefault(newIdx - 1);
        
            if (newAnnotation is null)
            {
                newAnnotation = _sortedFilteredAnnotationList.Last();
            }
        
            ChangeAnnotation(newAnnotation.GUID);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }

    [RelayCommand]
    private void ChangeAnnotation(Guid? newGUID)
    {
        if (SelectedAnnotation is not null)
        {
            if (newGUID is null)
            {
                SelectedAnnotation = null;
                return;
            }
            
            if (newGUID == Guid.Empty)
                return;
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            SelectedAnnotation = SortedFilteredAnnotationList.FirstOrDefault(avm => avm.GUID == newGUID);
        });
    }

    public void StartAnnotation(PointerPositionModel pointerPosition)
    {
        var bbox = new BoundingBox(
            pointerPosition.PX,
            pointerPosition.PY,
            pointerPosition.PX,
            pointerPosition.PY);
        var cls = _classListProvider.GetActiveClass();
        var idx = _imageSpace.AnnotationRunningCount;
        
        var annotation = new Annotation(bbox, cls, idx);
        
        if (_annotationList.Count > 0)
            _annotationList.Last().IsLastDrawn = false;
        
        var newAvm = _annotationViewModelFactory.Create(annotation,
            true);
        
        newAvm.IsLastDrawn = true;
        
        _annotationList.Add(newAvm);
        ChangeAnnotation(newAvm.GUID);
    }
    
    public void RedrawAnnotation(PointerPositionModel pointerPosition)
    {
        if (_annotationList.Count <= 0) return;

        SelectedAnnotation?.UpdateAnnotation(pointerPosition);
    }
    
    public bool FinishAnnotation(PointerPositionModel pointerPosition)
    {
        if (SelectedAnnotation is null)
            return false;
        
        return SelectedAnnotation.FinishAnnotation(pointerPosition);
    }
    
    [RelayCommand]
    private void RemoveAllBBoxes()
    {
        if (_annotationList.Count <= 0)
            return;
        
        try
        {
            var annotationDataList = _annotationList.Select(a => a.AnnotationData).ToList();
            _undoRedoService.Execute(new RemoveBatchAnnotationCommand(ImageSpace, annotationDataList));
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void RemoveLastBBox()
    {
        if (_annotationList.Count <= 0)
            return;
        
        try
        {
            var lastAnnotation = _annotationList.Last();
            _undoRedoService.Execute(new RemoveAnnotationCommand(ImageSpace, lastAnnotation.AnnotationData));
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    public override string ToString()
    {
        return ImageSpace.Source.ImageName;
    }

    private AnnotationViewModel CreateAnnotationViewModel(Annotation annotation, bool temporary = false)
    {
        var avm = _annotationViewModelFactory.Create(annotation,
            temporary);
        
        return avm;
    }
    
    private void RemoveAnnotationViewModel(Annotation annotation)
    {
        if (SelectedAnnotation is not null)
        {
            if (annotation == SelectedAnnotation.AnnotationData)
            {
                SelectedAnnotation = null;
            }
        }
        
        var avm = _annotationList.First(vm => vm.AnnotationData == annotation);
        avm.DeleteAnnotation();
        _annotationList.Remove(avm);
    }

    [RelayCommand]
    public void DeleteImage()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (_annotationList.Count == 0)
        {
            foreach (var annotation in _annotationList)
            {
                annotation.DeleteAnnotation();
                annotation.PropertyChanged -= OnAnnotationPropertyChanged;
            }
            
            _annotationList.CollectionChanged -= OnAnnotationListChanged;
            
            _annotationList.Clear();
            _sortedFilteredAnnotationList.Clear();
        }
        
        _annotationModes.PropertyChanged -= OnAnnotationModeChanged;
        ((INotifyCollectionChanged)_imageSpace.Annotations).CollectionChanged -= OnDomainChanged;
        ((INotifyCollectionChanged)_classList).CollectionChanged -= OnClassListCollectionChanged;
        
        WeakReferenceMessenger.Default.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }
}