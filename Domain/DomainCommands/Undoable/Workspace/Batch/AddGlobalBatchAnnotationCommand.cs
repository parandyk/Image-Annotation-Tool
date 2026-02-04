using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;

public class AddGlobalBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<AddBatchAnnotationCommand> _addBatchAnnotationCommands;
    
    public AddGlobalBatchAnnotationCommand(Dictionary<ImageSpace, Dictionary<BoundingBox, ClassData?>> imagesAnnotationsDict)
    {
        _addBatchAnnotationCommands = imagesAnnotationsDict.Select(
            kvp => new AddBatchAnnotationCommand(kvp.Key, kvp.Value)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _addBatchAnnotationCommands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        // foreach (var cmd in _addBatchAnnotationCommands)
        // {
        //     cmd.Undo();
        // }
        
        for (int i = _addBatchAnnotationCommands.Count - 1; i >= 0; i--)
        {
            _addBatchAnnotationCommands[i].Undo();
        }
    }
}