using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;

public class AddBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<AddAnnotationCommand> _addAnnotationCommands;
    
    public AddBatchAnnotationCommand(ImageSpace imageSpace, Dictionary<BoundingBox, ClassData> bboxesClasses)
    {
        _addAnnotationCommands = bboxesClasses.Select(
            kvp =>  new AddAnnotationCommand(imageSpace, kvp.Key, kvp.Value)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _addAnnotationCommands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _addAnnotationCommands.Count - 1; i >= 0; i--)
        {
            _addAnnotationCommands[i].Undo();
        }
    }
}