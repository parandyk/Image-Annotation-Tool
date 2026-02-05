using System.ComponentModel;

namespace ImageAnnotationTool.Enums;

public enum ClassFilterMode
{
    [Description("Show all")]
    None,
    
    [Description("Hide used")] 
    HideUsed,
    
    [Description("Hide unused")]
    HideUnused
}