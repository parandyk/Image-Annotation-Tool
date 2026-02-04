using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public abstract class WorkspaceCommand : IUndoableCommand //TODO FOR REMOVAL
{
    protected readonly AnnotationWorkspace _annotationWorkspace;

    protected WorkspaceCommand(AnnotationWorkspace annotationWorkspace)
    {
        _annotationWorkspace = annotationWorkspace;
    }
    
    public abstract void Execute();

    public abstract void Undo();
}