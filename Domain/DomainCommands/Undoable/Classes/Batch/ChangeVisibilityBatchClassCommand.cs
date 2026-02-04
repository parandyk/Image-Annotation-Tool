using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes.Batch;

public class ChangeVisibilityBatchClassCommand : IUndoableCommand
{
    private readonly List<ChangeVisibilityClassCommand> _commands;

    public ChangeVisibilityBatchClassCommand(List<ClassData> classDataList, bool? newVisibility)
    {
        _commands = classDataList.Select(
            classData => new ChangeVisibilityClassCommand(classData, newVisibility))
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