using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;

namespace ImageAnnotationTool.Domain.Infrastructure.UseCases;

public interface IExportUseCaseInterface
{
    void ExportAnnotations(AnnotationExportContext context);
    void ExportClasses(ClassExportContext context);
}