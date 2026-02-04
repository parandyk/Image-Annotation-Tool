using System.ComponentModel;

namespace ImageAnnotationTool.Enums;

public enum AnnotationAddingMode
{
    [Description("Click drawing")]
    ClickDraw,
    
    [Description("Drag drawing")]
    DragDraw
}