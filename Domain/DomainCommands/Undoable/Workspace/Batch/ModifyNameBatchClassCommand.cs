using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;

public class ModifyNameBatchClassCommand : IUndoableCommand
{
    private readonly List<ModifyNameClassCommand> _commands;
    
    public ModifyNameBatchClassCommand(AnnotationWorkspace annotationWorkspace, 
        IClassDataPolicy policy,
        Dictionary<string, ClassData> namesClassesDict)
    {
        if (namesClassesDict.Keys.Count < namesClassesDict.Values.Count)
        {
            throw new ConstraintException("Class names cannot repeat.", new ArgumentException(null, nameof(namesClassesDict.Keys)));
        }
        
        _commands = namesClassesDict.Select(
            kvp => new ModifyNameClassCommand(annotationWorkspace, policy, kvp.Value, kvp.Key)).ToList();
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