using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum ImageFilterMode
{
    [Description("Show all")]
    None,
    
    [Description("Hide annotated")]
    Annotated,
    
    [Description("Hide unannotated")]
    Unannotated
}