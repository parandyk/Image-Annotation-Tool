using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.UseCases;

public sealed class ExportAnnotationsUseCase
{
    private readonly Dictionary<AnnotationFormat, IAnnotationExportStrategy> _exporters;

    public ExportAnnotationsUseCase(IEnumerable<IAnnotationExportStrategy> exporters)
    {
        _exporters = exporters.ToDictionary(e 
            => e.Format, e => e);
    }
    
    public void Execute(AnnotationExportContext context)
    {
        var exporter = _exporters[context.Format];
        exporter.ExportAsync(context);
    }
}