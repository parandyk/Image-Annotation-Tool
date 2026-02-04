using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Infrastructure;
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
    public AnnotationAddingMode AddingMode { get; set; }
    
    // Rendering settings
    public bool DynamicBordersOn { get; set; }
    public bool BboxBorderOn { get; set; }
    public bool BboxBackgroundOn { get; set; }
    public bool OverridingDefaultBboxBorderThickness { get; set; }
    public double BboxBorderThickness { get; set; }
    
    // Notification settings
    public bool SuppressAnnotationDeletionDialogs { get; set; }
    public bool SuppressClassDeletionDialogs { get; set; }
    public bool SuppressImageDeletionDialogs { get; set; }
    
    // Interaction settings
    public bool OverridingDefaultDragThreshold { get; set; }
    public double DragThreshold { get; set; }
}