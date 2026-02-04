using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Images;

public sealed class AddAnnotationCommand : IUndoableCommand
{
    private readonly ImageSpace _imageSpace;
    private readonly Annotation _annotation;
    
    public AddAnnotationCommand(ImageSpace imageSpace, BoundingBox bbox, ClassData classData)
    {
        _imageSpace = imageSpace;
        _annotation = new Annotation(bbox, classData, imageSpace.AnnotationRunningCount);
        _imageSpace.IncrementAnnotationRunningCount();
    }
    
    public void Execute()
    {
        _imageSpace.AddAnnotation(_annotation);
    }

    public void Undo()
    {
        _imageSpace.RemoveAnnotation(_annotation);
    }
}