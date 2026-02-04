using System.ComponentModel;

namespace ImageAnnotationTool.Enums;

public enum ClassFilterMode
{
    [Description("Show all")]
    None,
    
    [Description("Show used")]
    Used,
    
    [Description("Show unused")]
    Unused
}