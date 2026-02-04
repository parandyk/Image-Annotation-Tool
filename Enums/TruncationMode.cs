using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum TruncationMode
{
    [Display(Name = "None")]
    None = 0,
    
    [Display(Name = "Start")]
    Start = 1,
    
    [Display(Name = "End")]
    End = 2,
    
    [Display(Name = "Middle")]
    Middle = 3
}