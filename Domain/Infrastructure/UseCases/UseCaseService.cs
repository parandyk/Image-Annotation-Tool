using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;

namespace ImageAnnotationTool.Domain.Infrastructure.UseCases;

public sealed class UseCaseService : IUseCaseProvider
{
    private readonly ExportAnnotationsUseCase _exportAnnotationsUseCase;
    private readonly ExportClassesUseCase _exportClassesUseCase;

    public UseCaseService(ExportAnnotationsUseCase exportAnnotationsUseCase,
        ExportClassesUseCase exportClassesUseCase)
    {
        _exportAnnotationsUseCase = exportAnnotationsUseCase;
        _exportClassesUseCase = exportClassesUseCase;
    }
    
    public void ExportAnnotations(AnnotationExportContext context)
    {
        _exportAnnotationsUseCase.Execute(context);
    }

    public void ExportClasses(ClassExportContext context)
    {
        _exportClassesUseCase.Execute(context);
    }

    public void ImportAnnotations()
    {
        throw new System.NotImplementedException();
    }

    public void ImportClasses()
    {
        throw new System.NotImplementedException();
    }

    public void ImportImages()
    {
        throw new System.NotImplementedException();
    }
}