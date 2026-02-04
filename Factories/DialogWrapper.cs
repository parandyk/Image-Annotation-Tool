using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Factories;

public class DialogWrapper : IDialogWrapper
{
    private INotifyPropertyChanged? _defaultOwnerVM;
    private readonly IClassListProvider _classListProvider;
    private readonly IStatisticsAggregator _statisticsAggregator;
    private readonly IClassDataPolicy _classDataPolicy;
    private readonly IDialogService _dialogService;
    private readonly IRenderingSettings _renderingSettings;
    private readonly IInteractionSettings _interactionSettings;
    private readonly INotificationSettings _notificationSettings;
    private readonly IAppModeSettings _appModeSettings;
    
    public DialogWrapper(IClassListProvider classListProvider, 
        IStatisticsAggregator statisticsAggregator, 
        IClassDataPolicy classDataPolicy,
        IDialogService dialogService,
        IRenderingSettings renderingSettings,
        IInteractionSettings interactionSettings,
        INotificationSettings notificationSettings,
        IAppModeSettings appModeSettings)
    {
        _classListProvider = classListProvider;
        _statisticsAggregator = statisticsAggregator;
        _classDataPolicy = classDataPolicy;
        _dialogService = dialogService;
        _renderingSettings = renderingSettings;
        _interactionSettings = interactionSettings;
        _notificationSettings = notificationSettings;
        _appModeSettings = appModeSettings;
    }
    
    public void SetDefaultOwner(INotifyPropertyChanged owner)
    {
        _defaultOwnerVM = owner;
    }
    
    public ChangeClassDialogViewModel CreateChangeAnnotationClassDialog(Annotation annotationToModify)
    {
        return new ChangeClassDialogViewModel(_classListProvider, _statisticsAggregator, annotationToModify);
    }

    public ChangeClassDialogViewModel CreateChangeAnnotationClassDialog(List<Annotation> annotationsToModify)
    {
        // return new ChangeClassDialogViewModel(_classListProvider, _statisticsAggregator, List<Annotation> annotationsToModify);
        throw new System.NotImplementedException();
    }
    
    public DeleteClassDialogViewModel CreateDeleteClassDialog(ClassData classToDelete, ClassData? substituteClass = null)
    {
        return new DeleteClassDialogViewModel(_classListProvider, _statisticsAggregator, classToDelete, substituteClass);
    }

    public DeleteClassDialogViewModel CreateDeleteClassDialog(List<ClassData> classesToDelete)
    {
        throw new System.NotImplementedException();
    }

    public RenameClassDialogViewModel CreateRenameClassDialog(ClassData classToRename)
    {
        return new RenameClassDialogViewModel(_classListProvider, _statisticsAggregator, _classDataPolicy, classToRename);
        // return new RenameClassDialogViewModel(_statisticsAggregator, classToRename);
    }

    public RenameClassDialogViewModel CreateRenameClassDialog(List<ClassData> classesToRename)
    {
        throw new NotImplementedException();
    }

    public SwapClassInstancesDialogViewModel CreateSwapClassInstancesDialog(ClassData classToSwap, ClassData? substituteClass = null, ImageSpace? image = null)
    {
        return new SwapClassInstancesDialogViewModel(_classListProvider, _statisticsAggregator, classToSwap, substituteClass, image);
    }

    public SwapClassInstancesDialogViewModel CreateSwapClassInstancesDialog(List<ClassData> classesToSwap)
    {
        throw new NotImplementedException();
    }

    public RemoveClassInstancesDialogViewModel CreateRemoveClassInstancesDialog(ClassData classToRemove, ImageSpace? image = null)
    {
        return new RemoveClassInstancesDialogViewModel(_statisticsAggregator, classToRemove, image);
    }

    public RemoveClassInstancesDialogViewModel CreateRemoveClassInstancesDialog(List<ClassData> classesToRemove)
    {
        throw new NotImplementedException();
    }

    public DeleteAnnotationDialogViewModel CreateDeleteAnnotationDialog(Annotation annotationToDelete)
    {
        return new DeleteAnnotationDialogViewModel(annotationToDelete);
    }

    public DeleteAnnotationDialogViewModel CreateDeleteAnnotationDialog(List<Annotation> annotationsToDelete)
    {
        // return new DeleteAnnotationDialogViewModel(annotationsToDelete);
        throw new NotImplementedException();
    }
    
    public DeleteImageDialogViewModel CreateDeleteImageDialog(ImageSpace imageToDelete)
    {
        return new DeleteImageDialogViewModel(imageToDelete);
    }
    
    public DeleteImageDialogViewModel CreateDeleteImageDialog(List<ImageSpace> imagesToDelete)
    {
        // return new DeleteImageDialogViewModel(imageToDelete);
        throw new NotImplementedException();
    }

    public Task<bool?> ShowDialogAsync(IModalDialogViewModel viewModel, INotifyPropertyChanged? ownerViewModel = null)
    {
        if (ownerViewModel == null)
        {
            ownerViewModel = _defaultOwnerVM ?? throw new InvalidOperationException("No owner was provided for the window.");
        }
        return _dialogService.ShowDialogAsync(ownerViewModel, viewModel);
    }

    public SettingsDialogViewModel CreateSettingsDialog()
    {
        return new SettingsDialogViewModel(_renderingSettings, _interactionSettings, _notificationSettings, _appModeSettings);
    }
}