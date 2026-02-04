using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public abstract class ImageCommand : IUndoableCommand
{
    protected readonly ImageSpace _imageSpace;

    protected ImageCommand(ImageSpace imageSpace)
    {
        _imageSpace = imageSpace;
    }
    
    public abstract void Execute();

    public abstract void Undo();
}