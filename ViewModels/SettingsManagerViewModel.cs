using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Interfaces;

namespace ImageAnnotationTool.ViewModels;

public partial class SettingsManagerViewModel : ObservableObject
{
    private readonly IDialogWrapper _dialogWrapper;
    private readonly IAppMessenger _messenger;
    private readonly ICommandStack _undoRedoService;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IRenderingSettings _renderingSettings;
    private readonly IInteractionSettings _interactionSettings;
    private readonly INotificationSettings _notificationSettings;
    private readonly IAppModeSettings _appModeSettings;
    private readonly IStatisticsAggregator _statisticsAggregator;

    public SettingsManagerViewModel(
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger,
        ICommandStack undoRedoService,
        IWorkspaceCommandFactory commandFactory,
        IRenderingSettings renderingSettings,
        IInteractionSettings interactionSettings,
        INotificationSettings notificationSettings,
        IAppModeSettings appModeSettings,
        IStatisticsAggregator statisticsAggregator)
    {
        _dialogWrapper = dialogWrapper;
        _messenger = messenger;
        _undoRedoService = undoRedoService;
        _commandFactory = commandFactory;
        _renderingSettings = renderingSettings;
        _interactionSettings = interactionSettings;
        _notificationSettings = notificationSettings;
        _appModeSettings = appModeSettings;
        _statisticsAggregator = statisticsAggregator;

        _statisticsAggregator.PropertyChanged += OnStatisticsAggregatorPropertyChanged;
    }

    private void OnStatisticsAggregatorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IStatisticsAggregator.GlobalClassVisibility))
        {
            OnPropertyChanged(nameof(GlobalClassVisibility));
        }
        
        else if (e.PropertyName == nameof(IStatisticsAggregator.GlobalAnnotationVisibility))
        {
            OnPropertyChanged(nameof(GlobalBBoxVisibility));
        }
        
        else if (e.PropertyName == nameof(IStatisticsAggregator.GlobalAnnotationAnchoring))
        {
            OnPropertyChanged(nameof(GlobalBBoxAnchoring));
        }
    }
    
    public double DragThreshold
    {
        get
        {
            if (_interactionSettings.OverridingDefaultDragThreshold) 
                return _interactionSettings.DragThreshold;

            return InteractionConstants.DefaultDragThreshold;
        }
        set
        {
            if (Equals(_interactionSettings.DragThreshold, value)) 
                return;
            
            _interactionSettings.SetDragThreshold(value);
            OnPropertyChanged();
        }
    }
    
    public double BboxBorderThickness
    {
        get
        {
            if (_renderingSettings.OverridingDefaultBboxBorderThickness) 
                return _renderingSettings.BboxBorderThickness;

            return RenderingConstants.DefaultThickness;
        }
        set
        {
            if (Equals(_renderingSettings.BboxBorderThickness, value)) 
                return;
            
            _renderingSettings.SetBboxBorderThickness(value);
            OnPropertyChanged();
        }
    }
    
    public bool DynamicBordersOn
    {
        get => _renderingSettings.DynamicBordersOn;
        set
        {
            if (Equals(_renderingSettings.DynamicBordersOn, value)) 
                return;

            _renderingSettings.SetDynamicBordersOn(value);
            OnPropertyChanged();
        }
    }
    
    public bool BboxBorderOn
    {
        get => _renderingSettings.BboxBorderOn;
        set
        {
            if (_renderingSettings.BboxBorderOn == value) 
                return;
            
            _renderingSettings.SetBboxBorderOn(value);
            OnPropertyChanged();
        }
    }
    
    public bool BboxBackgroundOn
    {
        get => _renderingSettings.BboxBackgroundOn;
        set
        {
            if (_renderingSettings.BboxBackgroundOn == value) 
                return;
            
            _renderingSettings.SetBboxBackgroundOn(value);
            OnPropertyChanged();
        }
    }
    
    public bool EditingModeOn
    {
        get => _appModeSettings.EditingModeOn;
        set
        {
            if (_appModeSettings.EditingModeOn == value) 
                return;
            
            _appModeSettings.SetEditingModeOn(value);
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private async Task OpenSettingsDialog() //TODO
    {
        try
        {
            var vm = _dialogWrapper.CreateSettingsDialog();
            var result = await _dialogWrapper.ShowDialogAsync(vm);

            if (result is true)
            {
                // SaveSettings(); //TODO
                _messenger.SendErrorOccurredNotification("Settings changes saved.");
            }
            else
            {
                _messenger.SendErrorOccurredNotification("Setting changes aborted.");
            }
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    public bool? GlobalClassVisibility
    {
        get => _statisticsAggregator.GlobalClassVisibility;
        
        set
        {
            try
            {
                bool newVisibility;
                
                if (value == null)
                {
                    newVisibility = !_renderingSettings.GlobalClassVisibility;
                    _renderingSettings.SetGlobalClassVisibility(newVisibility);
                    _undoRedoService.Execute(_commandFactory.ChangeGlobalClassVisibility(newVisibility));
                    OnPropertyChanged(nameof(GlobalClassVisibility));
                    return;
                }
    
                newVisibility = value.Value;
            
                if (_renderingSettings.GlobalClassVisibility == newVisibility)
                    return;
    
                _renderingSettings.SetGlobalClassVisibility(newVisibility);
                _undoRedoService.Execute(_commandFactory.ChangeGlobalClassVisibility(newVisibility));
                OnPropertyChanged(nameof(GlobalClassVisibility));
            }
            catch (Exception e)
            {
                _messenger.SendErrorOccurredNotification(e.Message);
            }
        }
    }
    
    public bool? GlobalBBoxVisibility
    {
        get => _statisticsAggregator.GlobalAnnotationVisibility;
        
        set
        {
            try
            {
                bool newVisibility;
                
                if (value == null)
                {
                    newVisibility = !_renderingSettings.GlobalAnnotationVisibility;
                    _renderingSettings.SetGlobalAnnotationVisibility(newVisibility);
                    _undoRedoService.Execute(_commandFactory.ChangeGlobalAnnotationVisibility(newVisibility));
                    OnPropertyChanged(nameof(GlobalBBoxVisibility));
                    return;
                }
    
                newVisibility = value.Value;
            
                if (_renderingSettings.GlobalAnnotationVisibility == newVisibility)
                    return;
    
                _renderingSettings.SetGlobalAnnotationVisibility(newVisibility);
                _undoRedoService.Execute(_commandFactory.ChangeGlobalAnnotationVisibility(newVisibility));
                OnPropertyChanged(nameof(GlobalBBoxVisibility));
            }
            catch (Exception e)
            {
                _messenger.SendErrorOccurredNotification(e.Message);
            }
        }
    }
    
    public bool? GlobalBBoxAnchoring
    {
        get => _statisticsAggregator.GlobalAnnotationAnchoring;
        
        set
        {
            try
            {
                bool newAnchoring;
                
                if (value == null)
                {
                    newAnchoring = !_interactionSettings.GlobalBBoxAnchoring;
                    _interactionSettings.SetGlobalBBoxAnchoring(newAnchoring);
                    _undoRedoService.Execute(_commandFactory.ChangeGlobalAnnotationAnchoring(newAnchoring));
                    OnPropertyChanged(nameof(GlobalBBoxAnchoring));
                    return;
                }
    
                newAnchoring = value.Value;
            
                if (_interactionSettings.GlobalBBoxAnchoring == newAnchoring)
                    return;
    
                _interactionSettings.SetGlobalBBoxAnchoring(newAnchoring);
                _undoRedoService.Execute(_commandFactory.ChangeGlobalAnnotationAnchoring(newAnchoring));
                OnPropertyChanged(nameof(GlobalBBoxAnchoring));
            }
            catch (Exception e)
            {
                _messenger.SendErrorOccurredNotification(e.Message);
            }
        }
    }

    public bool FilteredAnnotationVisibility //TODO
    {
        get => _renderingSettings.FilteredAnnotationVisibility;
        set
        {
            if (_renderingSettings.FilteredAnnotationVisibility == value) 
                return;
            
            _renderingSettings.SetFilteredAnnotationVisibility(value);
            OnPropertyChanged();
        }
    }
    
    public bool FilteredClassVisibility //TODO
    {
        get => _renderingSettings.FilteredClassVisibility;
        set
        {
            if (_renderingSettings.FilteredClassVisibility == value) 
                return;
            
            _renderingSettings.SetFilteredClassVisibility(value);
            OnPropertyChanged();
        }
    }
}