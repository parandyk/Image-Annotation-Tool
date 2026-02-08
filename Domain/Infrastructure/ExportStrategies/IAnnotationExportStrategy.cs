using System.Threading.Tasks;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;

public interface IAnnotationExportStrategy
{
    AnnotationFormat Format { get; }
    
    Task ExportAsync(AnnotationExportContext context);
}