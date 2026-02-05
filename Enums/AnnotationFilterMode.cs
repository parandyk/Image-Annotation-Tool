using System.ComponentModel;

namespace ImageAnnotationTool.Enums;

public enum AnnotationFilterMode
{
    [Description("Show all")]
    None,
    
    [Description("Hide assigned")]
    HideAssigned,
    
    [Description("Hide unassigned")]
    HideUnassigned
}