using System;

namespace ImageAnnotationTool.Domain.ValueObjects;

public struct BoundingBox
{
    public BoundingBox(double x1, double y1, double x2, double y2)
    {
        if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0)
        {
            throw new ArgumentOutOfRangeException(
                "Bounding box coordinates cannot be equal to, or less than zero.", 
                innerException: null);
        }
        
        X1 = x1 <= x2 ? x1 : x2;
        Y1 = y1 <= y2 ? y1 : y2;
        X2 = x2 >= x1 ? x2 : x1;
        Y2 = y2 >= y1 ? y2 : y1;
    }
    
    public double X1 { get; }
    public double Y1 { get; }
    
    public double X2 { get; }
    public double Y2 { get; }
    
    public double Width => X2 - X1; 
    public double Height => Y2 - Y1;
}