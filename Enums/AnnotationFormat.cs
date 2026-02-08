using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ImageAnnotationTool.Enums;

public enum AnnotationFormat
{
    [Display(Name="YOLO")]
    Yolo,
    
    [Display(Name="COCO")]
    Coco,
    
    [Display(Name="VOC")]
    Voc
}