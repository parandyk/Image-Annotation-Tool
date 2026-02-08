using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.Infrastructure.SettingsStore;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.ViewModels;

public partial class SettingsDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public SettingsDialogViewModel(IRenderingSettings renderingSettings,
        IInteractionSettings interactionSettings,
        INotificationSettings notificationSettings,
        IAppModeSettings appModeSettings)
    {
        AddingMode = appModeSettings.AddingMode;
        
        BboxBorderThickness = renderingSettings.BboxBorderThickness;
        OverridingDefaultBboxBorderThickness = renderingSettings.OverridingDefaultBboxBorderThickness;
        BboxBorderOn = renderingSettings.BboxBorderOn;
        BboxBackgroundOn = renderingSettings.BboxBackgroundOn;
        DynamicBordersOn = renderingSettings.DynamicBordersOn;
        
        SuppressAnnotationDeletionDialogs = notificationSettings.SuppressAnnotationDeletionDialogs;
        SuppressClassDeletionDialogs = notificationSettings.SuppressClassDeletionDialogs;
        SuppressImageDeletionDialogs = notificationSettings.SuppressImageDeletionDialogs;
        SuppressGlobalClassInstanceDeletionDialogs = notificationSettings.SuppressGlobalClassInstanceDeletionDialogs;
        SuppressLocalClassInstanceDeletionDialogs = notificationSettings.SuppressLocalClassInstanceDeletionDialogs;
        
        DragThreshold = interactionSettings.DragThreshold;
        OverridingDefaultDragThreshold = interactionSettings.OverridingDefaultDragThreshold;
    }
    
    [RelayCommand]
    private void Confirm()
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
    
    // App mode settings
    [ObservableProperty]
    private AnnotationAddingMode _addingMode;
    
    // Rendering settings
    [ObservableProperty]
    private bool _dynamicBordersOn;
    [ObservableProperty]
    private bool _bboxBorderOn;
    [ObservableProperty]
    private bool _bboxBackgroundOn;
    [ObservableProperty]
    private bool _overridingDefaultBboxBorderThickness;
    [ObservableProperty]
    private double _bboxBorderThickness;
    
    // Notification settings
    [ObservableProperty]
    private bool _suppressAnnotationDeletionDialogs;
    [ObservableProperty]
    private bool _suppressClassDeletionDialogs;
    [ObservableProperty]
    private bool _suppressImageDeletionDialogs;
    [ObservableProperty]
    private bool _suppressLocalClassInstanceDeletionDialogs;
    [ObservableProperty]
    private bool _suppressGlobalClassInstanceDeletionDialogs;
    
    // Interaction settings
    [ObservableProperty]
    private bool _overridingDefaultDragThreshold;
    [ObservableProperty]
    private double _dragThreshold;
}