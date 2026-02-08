using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.Entities;

public sealed class Annotation : ObservableObject
{
    public Annotation(BoundingBox bounds, ClassData classInfo, int displayId)
    {
        Bounds = bounds;
        ClassInfo = classInfo;
        DisplayId = displayId;
    }
    
    public event Action? AnnotationChanged;
    
    public Guid Id { get; } = Guid.NewGuid();
    
    public int DisplayId { get; }
    
    public ClassData ClassInfo { get; private set; }
    
    public BoundingBox Bounds { get; private set; }
    
    public bool IsVisible { get; private set; } = true;
    
    public bool IsAnchored { get; private set; } = false;
    
    internal void Move(double newX1, double newY1)
    {
        var xDelta = Bounds.X1 - newX1;
        var yDelta = Bounds.Y1 - newY1;
        
        SetBounds(newX1, newY1, Bounds.X1 + xDelta, Bounds.Y1 + yDelta);
    }

    internal void SetBounds(double x1, double y1, double x2, double y2)
    {
        Bounds = new BoundingBox(x1, y1, x2, y2);
        OnPropertyChanged(nameof(Bounds));
    }
    
    internal void SetBounds(BoundingBox newBounds)
    {
        Bounds = newBounds;
        OnPropertyChanged(nameof(Bounds));
    }

    internal void Resize(double newWidth, double newHeight)
    {
        double x1, x2, y1, y2;
        
        if (newWidth >= 0)
        {
            x1 = Bounds.X1;
            x2 = Bounds.X1 + newWidth;
        }
        else
        {
            x1 = Bounds.X1 + newWidth;
            x2 = Bounds.X1;
        }

        if (newHeight >= 0)
        {
            y1 = Bounds.Y1;
            y2 = Bounds.Y1 + newHeight;
        }
        else
        {
            y2 = Bounds.Y1;
            y1 = Bounds.Y1 + newHeight;
        }
        
        SetBounds(x1, y1, x2, y2);
    }

    internal void ChangeClass(ClassData newClassData)
    {
        ClassInfo = newClassData;
        OnPropertyChanged(nameof(ClassInfo));
        AnnotationChanged?.Invoke();
    }
    
    internal void ChangeVisibility(bool newState)
    {
        IsVisible = newState;
        OnPropertyChanged(nameof(IsVisible));
        AnnotationChanged?.Invoke();
    }
    
    internal void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        OnPropertyChanged(nameof(IsVisible));
        AnnotationChanged?.Invoke();
    }
    
    internal void ChangeAnchored(bool newState)
    {
        IsAnchored = newState;
        OnPropertyChanged(nameof(IsAnchored));
        AnnotationChanged?.Invoke();
    }
    
    internal void ToggleAnchored()
    {
        IsAnchored = !IsAnchored;
        OnPropertyChanged(nameof(IsAnchored));
        AnnotationChanged?.Invoke();
    }
}