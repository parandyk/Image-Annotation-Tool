using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public class ChangeGlobalVisibilityClassCommand : IUndoableCommand
{
    private readonly IUndoableCommand _command;
    
    public ChangeGlobalVisibilityClassCommand(AnnotationWorkspace annotationWorkspace, bool newVisibility)
    {
        var classes = annotationWorkspace.Classes.ToList();
        _command = new ChangeVisibilityBatchClassCommand(classes, newVisibility);
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