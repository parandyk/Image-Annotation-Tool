using System;
using ImageAnnotationTool.Domain.DataTransferObjects;

namespace ImageAnnotationTool.Domain.ExportableObjects;

public sealed class ExportableAnnotation
{
    private readonly AnnotationSnapshot _snapshot;
    private readonly double _imgWidth;
    private readonly double _imgHeight;

    public ExportableAnnotation(AnnotationSnapshot snapshot,
        double imgWidth, double imgHeight)
    {
        _snapshot = snapshot;
        _imgWidth = imgWidth;
        _imgHeight = imgHeight;
    }
    
    public string Label => _snapshot.Label;

    public (double cx, double cy, double w, double h) GetYoloCoordinates()
    {
        double cx = (_snapshot.X + _snapshot.Width / 2.0) / _imgWidth;
        double cy = (_snapshot.Y + _snapshot.Height / 2.0) / _imgHeight;
        double w = _snapshot.Width / _imgWidth;
        double h = _snapshot.Height / _imgHeight;
        
        return (Math.Clamp(cx, 0.0, 1.0), 
                Math.Clamp(cy, 0.0, 1.0),
                Math.Clamp(w, 0.0, 1.0), 
                Math.Clamp(h, 0.0, 1.0));
    }

    public (double x, double y, double w, double h) GetCocoCoordinates()
    {
        double x = _snapshot.X;
        double y = _snapshot.Y;
        double w = _snapshot.Width;
        double h = _snapshot.Height;
        
        return (Math.Clamp(x, 0.0, _imgWidth), 
            Math.Clamp(y, 0.0, _imgHeight),
            Math.Clamp(w, 0.0, _imgWidth), 
            Math.Clamp(h, 0.0, _imgHeight));
    }
}