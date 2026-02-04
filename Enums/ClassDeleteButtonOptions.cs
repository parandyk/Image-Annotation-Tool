using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum ClassDeleteButtonOptions
{
    [Display(Name="Delete affected")]
    DeleteAffected,
    
    [Display(Name="Swap to \"unassigned\"")]
    SwapToUnassigned,
    
    [Display(Name="Swap to specific")]
    SwapToSpecific,
    
    [Display(Name="Swap individually")] //TODO
    SwapIndividually
}