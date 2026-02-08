using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.DataTransferObjects;

namespace ImageAnnotationTool.Domain.Entities;

public sealed class ClassData : ObservableObject
{
    public ClassData(string className, string classHexColor)
    {
        Name = className;
        HexColor = classHexColor;
    }
    
    public event Action? ClassChanged;
    
    public Guid Id { get; } = Guid.NewGuid();
        
    public string Name { get; private set; }
    
    public string HexColor { get; private set; }
    
    public bool IsVisible { get; private set; } = true;
    
    internal void Rename(string newName)
    {
        Name = newName;
        OnPropertyChanged(nameof(Name));
        // ClassChanged?.Invoke();
    }

    internal void ChangeColor(string newHexColor)
    {
        HexColor = newHexColor;
        OnPropertyChanged(nameof(HexColor));
        // ClassChanged?.Invoke();
    }
    
    internal void ChangeVisibility(bool newState)
    {
        IsVisible = newState;
        OnPropertyChanged(nameof(IsVisible));
        ClassChanged?.Invoke();
    }
    
    internal void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        OnPropertyChanged(nameof(IsVisible));
        ClassChanged?.Invoke();
    }
}