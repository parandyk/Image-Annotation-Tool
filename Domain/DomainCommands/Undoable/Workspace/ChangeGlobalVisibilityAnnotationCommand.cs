using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations.Batch;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public class ChangeGlobalVisibilityAnnotationCommand : IUndoableCommand
{
    private readonly IUndoableCommand _command;
    
    public ChangeGlobalVisibilityAnnotationCommand(AnnotationWorkspace annotationWorkspace, bool newVisibility)
    {
        var annotations = annotationWorkspace.AllAnnotations.ToList();
        _command = new ChangeVisibilityBatchAnnotationCommand(annotations, newVisibility);
    }

    public void Execute()
    {
        _command.Execute();
    }

    public void Undo()
    {
        _command.Undo();
    }
}