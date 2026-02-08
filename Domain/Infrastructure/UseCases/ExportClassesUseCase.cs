using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.UseCases;

public sealed class ExportClassesUseCase
{
    private readonly Dictionary<ClassFormat, IClassExportStrategy> _exporters;

    public ExportClassesUseCase(IEnumerable<IClassExportStrategy> exporters)
    {
        _exporters = exporters.ToDictionary(e 
            => e.Format, e => e);
    }
    
    public void Execute(ClassExportContext context)
    {
        var exporter = _exporters[context.Format];
        exporter.ExportAsync(context);
    }
}