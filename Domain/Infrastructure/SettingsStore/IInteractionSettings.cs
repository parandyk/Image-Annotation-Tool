namespace ImageAnnotationTool.Domain.Infrastructure.SettingsStore;

public interface IInteractionSettings : IInteractionSettingsProvider
{
    
    void SetGlobalBBoxAnchoring(bool state);
    
    void UseDefaultDragThreshold(bool state);

    void SetDragThreshold(double threshold);
}