using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Core;
using static ImageAnnotationTool.Core.InteractionConstants;

namespace ImageAnnotationTool.Domain.Infrastructure;

public class InteractionSettings : ObservableObject, IInteractionSettings
{
    public double DragThreshold { get; private set; } = InteractionConstants.DefaultDragThreshold;
    public bool OverridingDefaultDragThreshold { get; private set; } = false;
    public bool GlobalBBoxAnchoring { get; private set; } = false;

    public void SetGlobalBBoxAnchoring(bool state)
    {
        if (GlobalBBoxAnchoring == state)
            return;
        
        GlobalBBoxAnchoring = state;
        OnPropertyChanged(nameof(GlobalBBoxAnchoring));
    }

    public void UseDefaultDragThreshold(bool state)
    {
        if (OverridingDefaultDragThreshold == state)
            return;
        
        OverridingDefaultDragThreshold = state;
        OnPropertyChanged(nameof(OverridingDefaultDragThreshold));
    }

    public void SetDragThreshold(double threshold)
    {
        if (Equals(DragThreshold, threshold))
            return;
        
        if (threshold < LowerDragThreshold)
            DragThreshold = LowerDragThreshold;
        else if (threshold > UpperDragThreshold)
            DragThreshold = UpperDragThreshold;
        else 
            DragThreshold = threshold;
        
        OnPropertyChanged(nameof(DragThreshold));
    }
}