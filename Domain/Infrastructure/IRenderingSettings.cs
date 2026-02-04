using System.ComponentModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IRenderingSettings : IRenderingSettingsProvider
{
    void UseDefaultBboxBorderThickness(bool state);
    
    void SetBboxBorderThickness(double thickness);
    void SetBboxBackgroundOn(bool state);
    void SetBboxBorderOn(bool state);
    void SetDynamicBordersOn(bool state);
    void SetGlobalBboxVisibility(bool state);
    void SetGlobalClassVisibility(bool state);
}