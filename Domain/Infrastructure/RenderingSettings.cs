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
    public bool GlobalBBoxVisibility { get; private set; } = true;
    public bool GlobalClassVisibility { get; private set; } = true;

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

    public void SetGlobalBboxVisibility(bool state)
    {
        if (GlobalBBoxVisibility == state)
            return;
        
        GlobalBBoxVisibility = state;
        OnPropertyChanged(nameof(GlobalBBoxVisibility));
    }
    
    public void SetGlobalClassVisibility(bool state)
    {
        if (GlobalClassVisibility == state)
            return;
        
        GlobalClassVisibility = state;
        OnPropertyChanged(nameof(GlobalClassVisibility));
    }
}