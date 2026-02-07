using System.Collections.Generic;
using ImageAnnotationTool.Domain.DataTransferObjects;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IAnnotationExporter
{
    void Export(
        IReadOnlyList<ImageWithAnnotations> imagesWithAnnotations,
        IReadOnlyDictionary<ClassSnapshot, int> classes,
        ExportContext context);
}