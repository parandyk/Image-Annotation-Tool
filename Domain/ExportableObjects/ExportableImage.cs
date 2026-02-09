using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.DataTransferObjects.General;

namespace ImageAnnotationTool.Domain.ExportableObjects;

public sealed class ExportableImage
{
    public string Filename { get; }
    public string Path { get; }
    public int Width { get; }
    public int Height { get; }
    public IEnumerable<ExportableAnnotation> Annotations { get; }

    public ExportableImage(ImageSnapshot snapshot)
    {
        Filename = snapshot.Name;
        Path = snapshot.Path;
        Width = snapshot.Width;
        Height = snapshot.Height;
        Annotations = snapshot.Annotations
            .Select(a => 
            new ExportableAnnotation(a, Width, Height))
            .ToList();
    }
}