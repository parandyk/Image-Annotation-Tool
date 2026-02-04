using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes.Batch;

public class ModifyColorBatchClassCommand : IUndoableCommand
{
    private readonly List<ModifyColorClassCommand>  _commands;

    public ModifyColorBatchClassCommand(IClassDataPolicy policy, Dictionary<string, ClassData> classes)
    {
        _commands = classes.Select(kvp => new ModifyColorClassCommand(policy, kvp.Value, kvp.Key)).ToList();
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