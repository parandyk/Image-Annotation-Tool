using System.Linq;
using Avalonia.Platform.Storage;

namespace ImageAnnotationTool.Models;

public static class FileExtensions
{
    public static readonly string[] ImageFiles = { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
    
    public static readonly string[] YoloClass = { ".txt", ".names", ".yaml" };
    public static readonly string[] YoloAnnotation = { ".txt" };
    
    public static readonly string[] Coco = { ".json" };
    
    public static readonly string[] PascalVoc = { ".xml" };
    
    public static readonly string[] AllClassFiles = YoloClass.Concat(Coco).Concat(PascalVoc).Distinct().ToArray();
    public static readonly string[] AllAnnotationFiles = YoloAnnotation.Concat(Coco).Concat(PascalVoc).ToArray();
    
    public static FilePickerFileType ToFilePickerFileType(string displayName, string[] extensions)
    {
        var patterns = extensions.Select(e => $"*.{e}").ToArray();
        return new FilePickerFileType(displayName)
        {
            Patterns = patterns
        };
    }
}