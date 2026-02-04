using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public class ChangeGlobalAnchoringAnnotationCommand : IUndoableCommand
{
    private readonly IUndoableCommand _command;
    
    public ChangeGlobalAnchoringAnnotationCommand(AnnotationWorkspace annotationWorkspace, bool newAnchoring)
    {
        var annotations = annotationWorkspace.AllAnnotations.ToList();
        _command = new ChangeAnchoringBatchAnnotationCommand(annotations, newAnchoring);
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