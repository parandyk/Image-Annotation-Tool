using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Annotations.Batch;

public class ChangeAnchoringBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<ChangeAnchoringAnnotationCommand> _commands;

    public ChangeAnchoringBatchAnnotationCommand(List<Annotation> annotations, bool? newAnchoring)
    {
        _commands = annotations.Select(
                annotation => new ChangeAnchoringAnnotationCommand(annotation, newAnchoring))
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