using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Standard.Batch;

public class AddBatchClassCommand : IPersistentCommand
{
    private readonly List<AddClassCommand> _addClassCommands;
    
    public AddBatchClassCommand(AnnotationWorkspace annotationWorkspace, IClassDataPolicy policy, List<string> classes)
    {
        _addClassCommands = classes.Select(c => new AddClassCommand(annotationWorkspace, policy, c)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _addClassCommands)
        {
            cmd.Execute();
        }
    }

    // public void Undo()
    // {
    //     for (int i = _addClassCommands.Count - 1; i >= 0; i--)
    //     {
    //         _addClassCommands[i].Undo();
    //     }
    // }
}