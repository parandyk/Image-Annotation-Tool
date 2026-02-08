using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum ClassFormat
{
    [Display(Name="txt")]
    Txt,
    
    [Display(Name="yaml")] 
    Yaml
}