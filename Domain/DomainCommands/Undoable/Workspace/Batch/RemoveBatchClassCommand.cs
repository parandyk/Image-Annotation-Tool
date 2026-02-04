using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;

public class RemoveBatchClassCommand : IUndoableCommand //TODO, CONSIDER REMOVING
{
    private readonly List<RemoveClassCommand> _commands;
    
    public RemoveBatchClassCommand(AnnotationWorkspace annotationWorkspace, 
        Dictionary<ClassData, ClassData?> deleteInsertClassDict)
    {
        _commands = deleteInsertClassDict.Select(kvp => 
            new RemoveClassCommand(annotationWorkspace, kvp.Key, kvp.Value)).ToList();
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