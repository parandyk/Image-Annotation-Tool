using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.Infrastructure.SettingsStore;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.ViewModels;

public partial class AnnotationViewModel : ObservableObject, IInteractiveElement, IDisposable
{
    private readonly Annotation _annotationData;
    private readonly ITransientAppModeSettings _modeSettings;
    private readonly IAppMessenger _messenger;
    
    public AnnotationViewModel(
        ITransientAppModeSettings modeSettings,
        IAppMessenger messenger,
        Annotation annotationData,
        bool temp = false)
    {
        _modeSettings = modeSettings;
        _messenger = messenger;

        Temporary = temp;

        _annotationData = annotationData;
        DisplayId = annotationData.DisplayId;

        LoadAnnotationData();
        
        _annotationData.PropertyChanged += OnAnnotationDataChanged;
        _modeSettings.PropertyChanged += OnAnnotationStateMetadataChanged;
    }

    public Action? Moved;

    private void OnAnnotationDataChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is Annotation annotation)
        {
            if (e.PropertyName == nameof(Annotation.ClassInfo))
            {
                OnPropertyChanged(nameof(AssignedClass));
                OnPropertyChanged(nameof(BBoxColorHex));
                OnPropertyChanged(nameof(BBoxDisplayName));
            }
            else if (e.PropertyName == nameof(Annotation.Bounds))
            {
                LoadAnnotationData();
            
                OnPropertyChanged(nameof(AnnotationData));
            }
            else if (e.PropertyName == nameof(Annotation.IsVisible))
            {
                OnPropertyChanged(nameof(IsVisible));
            }
            else if (e.PropertyName == nameof(Annotation.IsAnchored))
            {
                OnPropertyChanged(nameof(IsAnchored));
            }
        }
    }
    
    public bool Temporary = false;

    public ClassData AssignedClass => _annotationData.ClassInfo;
    
    public Annotation AnnotationData => _annotationData;
    
    public Guid GUID { get;} = Guid.NewGuid();
    
    public int DisplayId { get; init; }
    
    [ObservableProperty] 
    private double _x0;
    
    [ObservableProperty]
    private double _y0;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(X2))]
    private double _x;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Y2))]
    private double _y;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(X2))]
    private double _width;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Y2))]
    private double _height;

    public double X2 => X + Width;
    public double Y2 => Y + Height;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHighlighted))]
    [NotifyPropertyChangedFor(nameof(MovingEnabled))]
    [NotifyPropertyChangedFor(nameof(CanBeMoved))]
    private bool _isSelected;
    
    public bool IsAnchored => _annotationData.IsAnchored;
    
    [ObservableProperty]
    private bool _isLastDrawn;
    
    [ObservableProperty]
    private bool _previewingEnabled = true;
    
    public bool IsVisible => _annotationData.IsVisible;
   
    public string BBoxColorHex => AssignedClass.HexColor;
    
    public bool EditingModeOn => _modeSettings.EditingModeOn;
    
    public bool MovingEnabled => _modeSettings.EditingModeOn && IsSelected;

    public bool CanBeMoved => MovingEnabled && !IsAnchored;
    
    public bool IsHighlighted => IsSelected;
    
    public string BBoxDisplayName => string.Format(AnnotationNameFormat.AnnotationNameFormatString, DisplayId, _annotationData.ClassInfo.Name);
    
    private void OnAnnotationStateMetadataChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_modeSettings.EditingModeOn))
        {
            OnPropertyChanged(nameof(EditingModeOn));
            OnPropertyChanged(nameof(IsHighlighted));
            OnPropertyChanged(nameof(MovingEnabled));
            OnPropertyChanged(nameof(CanBeMoved));
        }
    }

    [RelayCommand]
    private void ChangeResizingState(bool isResizing)
    {
        TogglePreviewingMode(isResizing);
    }
    
    [RelayCommand]
    private void ChangeDraggingState(bool isDragging)
    {
        TogglePreviewingMode(isDragging);
    }

    private void TogglePreviewingMode(bool desiredState)
    {
        PreviewingEnabled = desiredState;
    }

    private void RevertCoordinates()
    {
        X0 = X = _annotationData.Bounds.X1;
        Y0 = Y = _annotationData.Bounds.Y1;
        Width = _annotationData.Bounds.Width;
        Height = _annotationData.Bounds.Height;
        
        Moved?.Invoke();
    }
    
    [RelayCommand]
    private void CancelPositionMutation()
    {
        RevertCoordinates();
        TogglePreviewingMode(false);
    }
    
    [RelayCommand]
    private void ResizeAnnotation(BoundingBox newBBox)
    {
        X0 = X = newBBox.X1;
        Y0 = Y = newBBox.Y1;
        Width = newBBox.Width;
        Height = newBBox.Height;
        
        Moved?.Invoke();
    }
    
    [RelayCommand]
    private void DragAnnotation(PointerPositionModel newPosition)
    {
        X0 = X = newPosition.PX;
        Y0 = Y = newPosition.PY;

        Moved?.Invoke();
    }

    private void LoadAnnotationData()
    {
        X0 = X = _annotationData.Bounds.X1;
        Y0 = Y = _annotationData.Bounds.Y1;
        
        Width = _annotationData.Bounds.Width;
        Height = _annotationData.Bounds.Height;
        
        Moved?.Invoke();
    }

    public void UpdateAnnotation(PointerPositionModel pointerPosition)
    {
        var px = pointerPosition.PX;
        var py = pointerPosition.PY;
        
        var tempWidth = double.Abs(px - X0);
        var tempHeight = double.Abs(py - Y0);
        
        X = X0 <= px ? X0 : px;
        Y = Y0 <= py ? Y0 : py;
    
        Width = tempWidth;
        Height = tempHeight;
        
        Moved?.Invoke();
    }
    
    public bool FinishAnnotation(PointerPositionModel pointerPosition)
    {
        if (Equals(pointerPosition.PX, X0) || Equals(pointerPosition.PY, Y0))
        {
            _messenger.SendErrorOccurredNotification("Box coordinates may not have the same components.");
            return false;
        }
        
        UpdateAnnotation(pointerPosition);
        return true;
    }
    
    [RelayCommand]
    public void DeleteAnnotation()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        _modeSettings.PropertyChanged -= OnAnnotationStateMetadataChanged;
        _annotationData.PropertyChanged -= OnAnnotationDataChanged;
        
        WeakReferenceMessenger.Default.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }
}