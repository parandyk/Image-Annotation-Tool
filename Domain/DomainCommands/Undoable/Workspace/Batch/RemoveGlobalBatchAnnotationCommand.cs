using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;

public class RemoveGlobalBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<RemoveBatchAnnotationCommand> _removeBatchAnnotationCommands;

    public RemoveGlobalBatchAnnotationCommand(Dictionary<ImageSpace, List<Annotation>> imagesAnnotationsDict)
    {
        _removeBatchAnnotationCommands = imagesAnnotationsDict.Select(
            kvp => new RemoveBatchAnnotationCommand(kvp.Key, kvp.Value)).ToList();
    }
    
    public RemoveGlobalBatchAnnotationCommand(AnnotationWorkspace annotationWorkspace, ClassData classData)
    {
        var imagesAnnotationsDict = annotationWorkspace.Images.Select(img => 
            new KeyValuePair<ImageSpace, List<Annotation>>(img, img.Annotations.Where(a => 
                a.ClassInfo == classData).ToList()))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
        _removeBatchAnnotationCommands = imagesAnnotationsDict.Select(
            kvp => new RemoveBatchAnnotationCommand(kvp.Key, kvp.Value)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _removeBatchAnnotationCommands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _removeBatchAnnotationCommands.Count - 1; i >= 0; i--)
        {
            _removeBatchAnnotationCommands[i].Undo();
        }
    }
}