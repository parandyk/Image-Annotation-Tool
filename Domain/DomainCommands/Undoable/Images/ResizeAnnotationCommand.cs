using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Images;

public class ResizeAnnotationCommand : IUndoableCommand
{
    private readonly ImageSpace _imageSpace;
    private readonly Annotation _annotation;
    private readonly BoundingBox _start;
    private readonly BoundingBox _finish;

    public ResizeAnnotationCommand(ImageSpace imageSpace, Annotation annotation, BoundingBox finish)
    {
        _imageSpace = imageSpace;
        _annotation = annotation;
        _start = annotation.Bounds;
        _finish = finish;
    }
    
    public void Execute()
    {
        _imageSpace.ResizeAnnotation(_annotation, _finish);
    }

    public void Undo()
    {
        _imageSpace.ResizeAnnotation(_annotation, _start);
    }
}