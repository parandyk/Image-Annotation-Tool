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
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Interfaces;
using IClassListProvider = ImageAnnotationTool.Domain.Infrastructure.IClassListProvider;
using IImageViewModelFactory = ImageAnnotationTool.Domain.Infrastructure.IImageViewModelFactory;

namespace ImageAnnotationTool.ViewModels;

public partial class ImageManagerViewModel : ObservableObject
{
    private readonly IWorkspaceDomainImageInterface _workspace;
    private readonly IImageViewModelFactory _imageViewModelFactory;
    private readonly IClassListProvider _classListProvider;
    private readonly INotificationSettings _notificationSettings;
    private readonly ICommandStack _undoRedoService;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IStatisticsAggregator _statisticsAggregator;
    private readonly IAppMessenger _messenger;
    private readonly IDialogWrapper _dialogWrapper;

    public ImageManagerViewModel(IWorkspaceDomainImageInterface workspace,
        IImageViewModelFactory imageViewModelFactory,
        IClassListProvider classListProvider,
        INotificationSettings notificationSettings,
        ICommandStack undoRedoService,
        IWorkspaceCommandFactory commandFactory,
        IStatisticsAggregator statisticsAggregator,
        IAppMessenger messenger,
        IDialogWrapper dialogWrapper)
    {
        _workspace = workspace;
        _imageViewModelFactory = imageViewModelFactory;
        _classListProvider = classListProvider;
        _undoRedoService = undoRedoService;
        _notificationSettings = notificationSettings;
        _commandFactory = commandFactory;
        _statisticsAggregator = statisticsAggregator;
        _messenger = messenger;
        _dialogWrapper = dialogWrapper;
        
        SortedFilteredImageViewModelList =
            new ReadOnlyObservableCollection<ImageViewModel>(_sortedFilteredImageViewModelList);
        
        RebuildImageDisplayList();
        
        _imageViewModelList.CollectionChanged += OnImageViewModelCollectionChanged;
        _sortedFilteredImageViewModelList.CollectionChanged += OnSortedFilteredImageViewModelCollectionChanged;
        ((INotifyCollectionChanged)_workspace.Images).CollectionChanged += OnDomainChanged;
    }

    private void OnSortedFilteredImageViewModelCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null || e.OldItems != null)
        {
            OnPropertyChanged(nameof(CanMoveForwardImage));
            OnPropertyChanged(nameof(CanMoveBackwardImage));
        }
    }

    private void OnDomainChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (ImageSpace img in e.OldItems)
            {
                RemoveImageViewModel(img);
            }
            
            var fallback = _imageViewModelList.LastOrDefault();

            if (fallback != null)
            {
                SelectedImageViewModel = fallback;
            }
        }

        if (e.NewItems != null)
        {
            foreach (ImageSpace img in e.NewItems)
            {
                if (_imageViewModelList.Any(ivm => ivm.ImageSpace == img))
                    continue;
                
                var ivm = CreateImageViewModel(img);
                _imageViewModelList.Add(ivm);
            }
            
            SelectedImageViewModel = _imageViewModelList.Last();
        }
    }
    
    private ImageViewModel CreateImageViewModel(ImageSpace img)
    {
        var ivm = _imageViewModelFactory.Create(img);
        return ivm;
    }
    
    private void RemoveImageViewModel(ImageSpace img)
    {
        if (SelectedImageViewModel is not null)
        {
            if (img == SelectedImageViewModel.ImageSpace)
            {
                SelectedImageViewModel = null;
            }
        }
        
        var ivm = _imageViewModelList.First(vm => vm.ImageSpace == img);
        
        ivm.DeleteImage();
        _imageViewModelList.Remove(ivm);
    }

    public bool BBoxesPresentGlobal => ImageViewModelList.Any(ivm => ivm.BBoxesPresentLocal);

    public bool CanMoveForwardImage => SelectedImageViewModel is not null && 
                                       _sortedFilteredImageViewModelList.Count > 1 &&
                                       _sortedFilteredImageViewModelList.IndexOf(SelectedImageViewModel) < 
                                       _sortedFilteredImageViewModelList.IndexOf(_sortedFilteredImageViewModelList.Last());

    public bool CanMoveBackwardImage => SelectedImageViewModel is not null &&
                                        _sortedFilteredImageViewModelList.Count > 1 &&
                                        _sortedFilteredImageViewModelList.IndexOf(SelectedImageViewModel) > 
                                        _sortedFilteredImageViewModelList.IndexOf(_sortedFilteredImageViewModelList.First());
    
    [ObservableProperty] 
    private ImageSortMode _imageSorting = ImageSortMode.None;

    [ObservableProperty] 
    private ImageFilterMode _imageFiltering = ImageFilterMode.None;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanMoveForwardImage))]
    [NotifyPropertyChangedFor(nameof(CanMoveBackwardImage))]
    private ImageViewModel? _selectedImageViewModel;
    
    private ObservableCollection<ImageViewModel> _imageViewModelList = [];
    
    public ReadOnlyObservableCollection<ImageViewModel> ImageViewModelList => 
        new ReadOnlyObservableCollection<ImageViewModel>(_imageViewModelList);

    private readonly ObservableCollection<ImageViewModel> _sortedFilteredImageViewModelList = new ObservableCollection<ImageViewModel>();

    public ReadOnlyObservableCollection<ImageViewModel> SortedFilteredImageViewModelList { get; }
    
    private void OnImageViewModelCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null || e.NewItems != null)
        {
            if (e.NewItems != null)
            {
                foreach (ImageViewModel ivm in e.NewItems)
                {
                    ivm.PropertyChanged += OnImageViewModelPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ImageViewModel ivm in e.OldItems)
                {
                    ivm.PropertyChanged -= OnImageViewModelPropertyChanged;
                }
            }
            
            RebuildImageDisplayList();
        }
    }

    private void OnImageViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ImageViewModel)
        {
            if (e.PropertyName == nameof(ImageViewModel.BBoxesPresentLocal))
            {
                OnPropertyChanged(nameof(BBoxesPresentGlobal));
                
                RebuildImageDisplayList();
            }
        }
    }

    partial void OnSelectedImageViewModelChanged(ImageViewModel? oldValue, ImageViewModel? newValue) //TODO: KEEP?
    {
        if (oldValue != null)
        {
            oldValue.IsSelected = false;
            
            if (oldValue.AnnotatingOn)
            {
                if (oldValue.AbortDrawingCommand.CanExecute(null)) 
                    oldValue.AbortDrawingCommand.Execute(null);
            }
        }
        
        if (newValue != null)
        {
            newValue.IsSelected = true;
        }
    }
    
    partial void OnImageSortingChanged(ImageSortMode oldValue, ImageSortMode newValue)
    {
        RebuildImageDisplayList();
    }
    
    partial void OnImageFilteringChanged(ImageFilterMode oldValue, ImageFilterMode newValue)
    {
        RebuildImageDisplayList();
    }

    /// Synchronous methods

    private void RebuildImageDisplayList()
    {
        List<ImageViewModel>? filteredTempList;

        switch (ImageFiltering)
        {
            default:
            case ImageFilterMode.None:
                filteredTempList = ImageViewModelList.ToList();
                break;
            case ImageFilterMode.Annotated:
                filteredTempList = ImageViewModelList.Where(ivm => !ivm.BBoxesPresentLocal).ToList();
                break;
            case ImageFilterMode.Unannotated:
                filteredTempList = ImageViewModelList.Where(ivm => ivm.BBoxesPresentLocal).ToList();
                break;
        }

        List<ImageViewModel>? sortedFilteredTempList;

        switch (ImageSorting)
        {
            default:
            case ImageSortMode.None:
                sortedFilteredTempList = filteredTempList.ToList();
                break;
            case ImageSortMode.Oldest: //TODO: COULD BE BROKEN
                sortedFilteredTempList = _imageViewModelList.IndexOfMany(filteredTempList)
                    .OrderBy(ivm => ivm.Index)
                    .Select(ivm => ivm.Item).ToList();
                break;
            case ImageSortMode.Newest: //TODO: COULD BE BROKEN
                sortedFilteredTempList = _imageViewModelList.IndexOfMany(filteredTempList)
                    .OrderByDescending(ivm => ivm.Index)
                    .Select(ivm => ivm.Item).ToList();
                break;
            case ImageSortMode.Alphabetical:
                sortedFilteredTempList = filteredTempList
                    .OrderBy(ivm => ivm.ImageSpace.Source.ImageName).ToList();
                break;
            case ImageSortMode.ReversedAlphabetical:
                sortedFilteredTempList = filteredTempList
                    .OrderByDescending(ivm => ivm.ImageSpace.Source.ImageName).ToList();
                break;
            case ImageSortMode.LargestFirst:
                sortedFilteredTempList = filteredTempList
                    .OrderByDescending(ivm => ivm.ImageSpace.Metadata.ImagePixelHeight 
                                              * ivm.ImageSpace.Metadata.ImagePixelWidth)
                    .ToList();
                break;
            case ImageSortMode.SmallestFirst:
                sortedFilteredTempList = filteredTempList
                    .OrderBy(ivm => ivm.ImageSpace.Metadata.ImagePixelHeight 
                                    * ivm.ImageSpace.Metadata.ImagePixelWidth)
                    .ToList();
                break;
            case ImageSortMode.MostAnnotations:
                sortedFilteredTempList = [];
                foreach (var (imgGuid, _) in _statisticsAggregator.AnnotationsPerImageCounts)
                {
                    sortedFilteredTempList.Add(filteredTempList.First(ivm => ivm.GUID == imgGuid));
                }
                break;
            case ImageSortMode.FewestAnnotations:
                sortedFilteredTempList = [];
                foreach (var (imgGuid, _) in _statisticsAggregator.AnnotationsPerImageCounts)
                {
                    sortedFilteredTempList.Insert(0, filteredTempList.First(ivm => ivm.GUID == imgGuid));
                }
                break;
            case ImageSortMode.MostTotalClasses:
                sortedFilteredTempList = [];
                foreach (var (imgGuid, _) in _statisticsAggregator.ClassesPerImageCounts)
                {
                    sortedFilteredTempList.Add(filteredTempList.First(ivm => ivm.GUID == imgGuid));
                }
                break;
            case ImageSortMode.FewestTotalClasses:
                sortedFilteredTempList = [];
                foreach (var (imgGuid, _) in _statisticsAggregator.ClassesPerImageCounts)
                {
                    sortedFilteredTempList.Insert(0, filteredTempList.First(ivm => ivm.GUID == imgGuid));
                }
                break;
            case ImageSortMode.ExtensionAlphabetical:
                sortedFilteredTempList = filteredTempList
                    .OrderBy(ivm => ivm.ImageSpace.Source.ImageExt)
                    .ToList();
                break;
            case ImageSortMode.ExtensionReversedAlphabetical:
                sortedFilteredTempList = filteredTempList
                    .OrderByDescending(ivm => ivm.ImageSpace.Source.ImageExt)
                    .ToList();
                break;
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            _sortedFilteredImageViewModelList.Clear();
            
            foreach (var item in sortedFilteredTempList)
            {
                _sortedFilteredImageViewModelList.Add(item);
            }
        });
        
        OnPropertyChanged(nameof(CanMoveForwardImage));
        OnPropertyChanged(nameof(CanMoveBackwardImage));
    }
    
    [RelayCommand]
    private async Task RemoveClassInstances(Guid guid)
    {
        if (_statisticsAggregator.AnnotationsPerClassCounts.FirstOrDefault(kvp => kvp.Key == guid).Value == 0)
        {
            _messenger.SendErrorOccurredNotification("No instances were found.");
            return;
        }

        try
        {
            var classData = _classListProvider.GetClass(guid);

            var vm = _dialogWrapper.CreateRemoveClassInstancesDialog(classData);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                _undoRedoService.Execute(_commandFactory.RemoveGlobalClassAnnotation(classData));
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
    private async Task SwapClassInstances(Guid guid)
    {
        if (_statisticsAggregator.AnnotationsPerClassCounts.FirstOrDefault(kvp => kvp.Key == guid).Value == 0)
        {
            _messenger.SendErrorOccurredNotification("No instances were found.");
            return;
        }

        try
        {
            var classData = _classListProvider.GetClass(guid);

            var vm = _dialogWrapper.CreateSwapClassInstancesDialog(classData);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                _undoRedoService.Execute(_commandFactory.ChangeClassBatchAnnotation(classData, vm.SubstituteClass));
                _messenger.SendErrorOccurredNotification("Class instances swapped successfully.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Class instances swapping cancelled.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void MoveImageForward()
    {
        try
        {
            if (_sortedFilteredImageViewModelList.Count <= 0) 
                return;
        
            if (SelectedImageViewModel is null) 
                return;
            
            var newIdx = _sortedFilteredImageViewModelList.IndexOf(SelectedImageViewModel);
            var newImage = _sortedFilteredImageViewModelList.ElementAtOrDefault(newIdx + 1);
        
            if (newImage is null)
            {
                newImage = _sortedFilteredImageViewModelList.First();
            }
        
            ChangeImage(newImage.GUID);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void MoveImageBackward()
    {
        try
        {
            if (_sortedFilteredImageViewModelList.Count <= 0) 
                return;

            if (SelectedImageViewModel is null) 
                return;
            
            var newIdx = _sortedFilteredImageViewModelList.IndexOf(SelectedImageViewModel);
            var newImage = _sortedFilteredImageViewModelList.ElementAtOrDefault(newIdx - 1);
        
            if (newImage is null)
            {
                newImage = _sortedFilteredImageViewModelList.Last();
            }
        
            ChangeImage(newImage.GUID);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private async Task DeleteImage(Guid imageGuid)
    {
        try
        {
            var image = _imageViewModelList.FirstOrDefault(i => i.GUID == imageGuid);
            
            if (image is null)
            {
                _messenger.SendErrorOccurredNotification("Image not found.");
                return;
            }
            
            var imageSpace = image.ImageSpace;
            
            if (!_workspace.ImageExists(imageSpace))
            {
                _messenger.SendErrorOccurredNotification("Image not found.");
                return;
            }
            
            if (_notificationSettings.SuppressImageDeletionDialogs)
            {
                _undoRedoService.Execute(_commandFactory.RemoveImage(imageSpace));
                _messenger.SendErrorOccurredNotification("Image deleted successfully.");
                return;
            }
            
            var vm = _dialogWrapper.CreateDeleteImageDialog(imageSpace);
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                if (vm.DontAskAgain)
                    _notificationSettings.ChangeSuppressionImageDeletionDialogs(true);
                
                _undoRedoService.Execute(_commandFactory.RemoveImage(imageSpace));
                _messenger.SendErrorOccurredNotification("Image deleted successfully.");
                return;
            }
            
            _messenger.SendErrorOccurredNotification("Image deletion cancelled.");
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand]
    private void OpenLastImage()
    {
        SelectedImageViewModel = ImageViewModelList.LastOrDefault();

        if (SelectedImageViewModel is null)
        {
            _messenger.SendErrorOccurredNotification("No image found.");
        }
    }
    
    [RelayCommand]
    private void OpenFirstImage()
    {
        SelectedImageViewModel = ImageViewModelList.FirstOrDefault();

        if (SelectedImageViewModel is null)
        {
            _messenger.SendErrorOccurredNotification("No image found.");
        }
    }
    
    [RelayCommand]
    private void ChangeImage(Guid? newIndex)
    {
        if (SelectedImageViewModel is not null)
        {
            if (newIndex is null)
            {
                SelectedImageViewModel = null;
                return;
            }
            
            if (newIndex == Guid.Empty)
                return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            SelectedImageViewModel = SortedFilteredImageViewModelList.FirstOrDefault(avm => avm.GUID == newIndex);
        });
    }

    [RelayCommand]
    private void AddImage(List<ValueTuple<ImageSource, ImageMetadata>> imageTuplesList)  //TODO: ADD DIALOG FOR NAME DUPLICATES
    {
        try
        {   if (imageTuplesList.Count > 0)
            {
                var imageDataDict = imageTuplesList.Select(img => 
                    new ValueTuple<ImageSource, ImageMetadata>(img.Item1, img.Item2)).ToDictionary();
                _undoRedoService.Execute(_commandFactory.AddBatchImage(imageDataDict));
            }
            else
            {
                var imageSource = imageTuplesList.First().Item1;
                var imageDimensions = imageTuplesList.First().Item2;
                _undoRedoService.Execute(_commandFactory.AddImage(imageSource, imageDimensions));
            }

        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
}