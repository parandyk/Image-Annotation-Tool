using System.Collections.Generic;
using ImageAnnotationTool.Domain.DataTransferObjects;

namespace ImageAnnotationTool.Domain.Infrastructure;

public sealed class ExportAnnotationsUseCase
{
    private readonly IAnnotationExporter _exporter;

    public ExportAnnotationsUseCase(IAnnotationExporter exporter)
    {
        _exporter = exporter;
    }
    
    public void Execute(
        IReadOnlyList<ImageWithAnnotations> imagesWithAnnotations,
        IReadOnlyDictionary<ClassSnapshot, int> classes,
        ExportContext context)
    {
        _exporter.Export(imagesWithAnnotations, classes, context);
    }
}