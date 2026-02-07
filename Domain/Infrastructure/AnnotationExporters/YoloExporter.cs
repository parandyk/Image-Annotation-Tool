using System.Collections.Generic;
using ImageAnnotationTool.Domain.DataTransferObjects;

namespace ImageAnnotationTool.Domain.Infrastructure.AnnotationExporters;

public sealed class YoloExporter : IAnnotationExporter
{
    public void Export(
        IReadOnlyList<ImageWithAnnotations> imagesWithAnnotations,
        IReadOnlyDictionary<ClassSnapshot, int> classes,
        ExportContext context)
    {
        
    }
}