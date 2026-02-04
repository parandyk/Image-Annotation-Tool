using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IInteractionSettings : IInteractionSettingsProvider
{
    
    void SetGlobalBBoxAnchoring(bool state);
    
    void UseDefaultDragThreshold(bool state);

    void SetDragThreshold(double threshold);
}