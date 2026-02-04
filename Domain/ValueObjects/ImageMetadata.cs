using System;
using System.Numerics;

namespace ImageAnnotationTool.Domain.ValueObjects;

public struct ImageMetadata
{
    public ImageMetadata(int imgPxWidth, int imgPxHeight, double dpiX, double dpiY, double aspectRatio)
    {
        if (imgPxWidth <= 0 || imgPxHeight <= 0)
        {
            throw new ArgumentOutOfRangeException("Image width and height must be greater than zero.", innerException: null);
        }

        if (aspectRatio <= 0)
        {
            throw new ArgumentOutOfRangeException("Aspect ratio must be greater than zero.", innerException: null);
        }
        
        if (dpiX <= 0 || dpiY <= 0)
        {
            DpiX = 96;
            DpiY = 96;
        }
        else
        {
            DpiX = dpiX;
            DpiY = dpiY;
        }
            
        ImagePixelWidth = imgPxWidth;
        ImagePixelHeight = imgPxHeight;
        AspectRatio = aspectRatio;
    }
    
    
    public int ImagePixelWidth { get; }
    public int ImagePixelHeight { get; }
    
    public double DpiX { get; }
    public double DpiY { get; }
    
    public double AspectRatio { get; }
}