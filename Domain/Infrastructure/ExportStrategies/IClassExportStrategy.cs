using System.Threading.Tasks;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.Domain.Infrastructure.ExportStrategies;

public interface IClassExportStrategy
{
    ClassFormat Format { get; }
    
    Task ExportAsync(ClassExportContext context);
}