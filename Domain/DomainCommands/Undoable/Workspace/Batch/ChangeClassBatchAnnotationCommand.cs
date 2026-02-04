using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;

public class ChangeClassBatchAnnotationCommand : IUndoableCommand
{
    private readonly List<ChangeClassAnnotationCommand> _changeClassAnnotationCommands;
    
    public ChangeClassBatchAnnotationCommand(AnnotationWorkspace annotationWorkspace, Dictionary<Annotation, ClassData?> classesAnnotationsDict)
    {
        _changeClassAnnotationCommands = classesAnnotationsDict.Select(
            kvp => new ChangeClassAnnotationCommand(annotationWorkspace,  kvp.Key, kvp.Value))
            .ToList();
    }
    
    public ChangeClassBatchAnnotationCommand(AnnotationWorkspace annotationWorkspace, List<Annotation> annotations, ClassData? newClass)
    {
        _changeClassAnnotationCommands = annotations.Select(
            annotation => new ChangeClassAnnotationCommand(annotationWorkspace, annotation, newClass))
            .ToList();
    }
    
    public ChangeClassBatchAnnotationCommand(AnnotationWorkspace annotationWorkspace, ClassData oldClass, ClassData? newClass)
    {
        var annotations = annotationWorkspace.Images.SelectMany(img => img.Annotations.Where(a => a.ClassInfo == oldClass)).ToList();
        _changeClassAnnotationCommands = annotations.Select(
            annotation => new ChangeClassAnnotationCommand(annotationWorkspace, annotation, newClass)).ToList();
    }
    
    public void Execute()
    {
        foreach (var cmd in _changeClassAnnotationCommands)
        {
            cmd.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _changeClassAnnotationCommands.Count - 1; i >= 0; i--)
        {
            _changeClassAnnotationCommands[i].Undo();
        }
    }
}