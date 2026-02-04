using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Core;
using static ImageAnnotationTool.Core.RenderingConstants;

namespace ImageAnnotationTool.Domain.Infrastructure;

public class RenderingSettings : ObservableObject, IRenderingSettings
{
    public bool OverridingDefaultBboxBorderThickness { get; private set; } = false;
    public double BboxBorderThickness { get; private set; } = RenderingConstants.DefaultThickness;
    public bool BboxBackgroundOn { get; private set; } = true;
    public bool BboxBorderOn { get; private set; } = true;
    public bool DynamicBordersOn { get; private set; } = false;
    public bool GlobalAnnotationVisibility { get; private set; } = true;
    public bool GlobalClassVisibility { get; private set; } = true;
    public bool FilteredAnnotationVisibility { get; private set; } = true;
    public bool FilteredClassVisibility { get; private set; } = true;

    public void UseDefaultBboxBorderThickness(bool state)
    {
        if (OverridingDefaultBboxBorderThickness == state)
            return;
        
        OverridingDefaultBboxBorderThickness = state;
        OnPropertyChanged(nameof(OverridingDefaultBboxBorderThickness));
    }

    public void SetBboxBorderThickness(double thickness)
    {
        if (Equals(BboxBorderThickness, thickness))
            return;
        
        if (thickness < MinThickness)
            BboxBorderThickness = MinThickness;
        else if (thickness > MaxThickness)
            BboxBorderThickness = MaxThickness;
        else 
            BboxBorderThickness = thickness;
        
        OnPropertyChanged(nameof(BboxBorderThickness));
    }

    public void SetBboxBackgroundOn(bool state)
    {
        if (BboxBackgroundOn == state)
            return;
        
        BboxBackgroundOn = state;
        OnPropertyChanged(nameof(BboxBackgroundOn));
    }

    public void SetBboxBorderOn(bool state)
    {
        if (BboxBorderOn == state)
            return;
        
        BboxBorderOn = state;
        OnPropertyChanged(nameof(BboxBorderOn));
    }

    public void SetDynamicBordersOn(bool state)
    {
        if (DynamicBordersOn == state)
            return;
        
        DynamicBordersOn = state;
        OnPropertyChanged(nameof(DynamicBordersOn));
    }

    public void SetGlobalAnnotationVisibility(bool state)
    {
        if (GlobalAnnotationVisibility == state)
            return;
        
        GlobalAnnotationVisibility = state;
        OnPropertyChanged(nameof(GlobalAnnotationVisibility));
    }
    
    public void SetGlobalClassVisibility(bool state)
    {
        if (GlobalClassVisibility == state)
            return;
        
        GlobalClassVisibility = state;
        OnPropertyChanged(nameof(GlobalClassVisibility));
    }

    public void SetFilteredAnnotationVisibility(bool state)
    {
        if (FilteredAnnotationVisibility == state)
            return;
        
        FilteredAnnotationVisibility = state;
        OnPropertyChanged(nameof(FilteredAnnotationVisibility));
    }

    public void SetFilteredClassVisibility(bool state)
    {
        if (FilteredClassVisibility == state)
            return;
        
        FilteredClassVisibility = state;
        OnPropertyChanged(nameof(FilteredClassVisibility));
    }
}