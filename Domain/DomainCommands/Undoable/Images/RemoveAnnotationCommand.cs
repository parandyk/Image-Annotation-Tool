using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Images;

public sealed class RemoveAnnotationCommand : IUndoableCommand
{
    private readonly ImageSpace _imageSpace;
    private readonly Annotation _annotation;
    
    public RemoveAnnotationCommand(ImageSpace imageSpace, Annotation annotation)
    {
        _imageSpace = imageSpace;
        _annotation = annotation;
    }
    
    public void Execute()
    {
        _imageSpace.RemoveAnnotation(_annotation);
    }

    public void Undo()
    {
        _imageSpace.AddAnnotation(_annotation);
    }
}