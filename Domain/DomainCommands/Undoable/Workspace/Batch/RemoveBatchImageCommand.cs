using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;

public class RemoveBatchImageCommand : IUndoableCommand
{
    private readonly List<RemoveImageCommand> _removeImageCommands;
    
    public RemoveBatchImageCommand(AnnotationWorkspace annotationWorkspace, List<ImageSpace> images)
    {
        _removeImageCommands = images.Select(img => 
                new RemoveImageCommand(annotationWorkspace, img)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _removeImageCommands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _removeImageCommands.Count - 1; i >= 0; i--)
        {
            _removeImageCommands[i].Undo();
        }
    }
}