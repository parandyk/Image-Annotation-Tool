namespace ImageAnnotationTool.Interfaces;

public interface IInteractiveElement
{
    double X { get; set; }
    double Y { get; set; }
    double Width { get; set; }
    double Height { get; set; }
    
    bool IsSelected { get; set;  }
    bool PreviewingEnabled { get; set;  }
    bool MovingEnabled { get; }
}