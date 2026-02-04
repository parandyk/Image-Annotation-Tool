using System;
using System.IO;

namespace ImageAnnotationTool.Domain.ValueObjects;

public sealed class ImageSource
{
    public ImageSource(string imgPath)
    {
        if (String.IsNullOrEmpty(imgPath))
        {
            throw new ArgumentOutOfRangeException(nameof(imgPath), "Image path cannot be empty");
        }
        
        ImagePath = imgPath;
        ImageName = Path.GetFileName(ImagePath);
        ImageExt = Path.GetExtension(ImagePath);
    }
    
    public string ImagePath { get; }
    public string ImageName { get; }
    public string ImageExt { get; }
    
}