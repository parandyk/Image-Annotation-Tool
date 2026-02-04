using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations.Batch;

public class ChangeVisibilityBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<ChangeVisibilityAnnotationCommand> _commands;

    public ChangeVisibilityBatchAnnotationCommand(List<Annotation> annotations, bool? newVisibility)
    {
        _commands = annotations.Select(
            annotation => new ChangeVisibilityAnnotationCommand(annotation, newVisibility))
            .ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _commands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }
}