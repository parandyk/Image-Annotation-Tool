using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;

public class RemoveBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<RemoveAnnotationCommand> _removeAnnotationCommands; 
        
    public RemoveBatchAnnotationCommand(ImageSpace imageSpace, List<Annotation> annotations)
    {
        _removeAnnotationCommands = annotations.Select(
            annotation => new RemoveAnnotationCommand(imageSpace, annotation)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _removeAnnotationCommands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _removeAnnotationCommands.Count - 1; i >= 0; i--)
        {
            _removeAnnotationCommands[i].Undo();
        }
    }
}