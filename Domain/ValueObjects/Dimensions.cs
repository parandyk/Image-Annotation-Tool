using System;

namespace ImageAnnotationTool.Domain.ValueObjects;

public struct Dimensions
{
    public Dimensions(double width, double height)
    {
        if (width < 0 || height < 0)
        {
            throw new ArgumentOutOfRangeException(
                "Dimensions cannot be less than zero.", 
                innerException: null);
        }
        
        Width = width;
        Height = height;
    }
    
    public double Width { get; }
    public double Height { get; }
}