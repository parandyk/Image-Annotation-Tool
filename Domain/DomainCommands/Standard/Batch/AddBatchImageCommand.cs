using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DomainCommands.Standard.Batch;

public class AddBatchImageCommand : IPersistentCommand
{
    private readonly List<AddImageCommand> _addImageCommands;
    
    public AddBatchImageCommand(AnnotationWorkspace annotationWorkspace, Dictionary<ImageSource, ImageMetadata> images)
    {
        _addImageCommands = images.Select(img => new AddImageCommand(annotationWorkspace, img.Key, img.Value)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _addImageCommands)
        {
            cmd.Execute();
        }
    }

    // public void Undo()
    // {
    //     for (int i = _addImageCommands.Count - 1; i >= 0; i--)
    //     {
    //         _addImageCommands[i].Undo();
    //     }
    // }
}