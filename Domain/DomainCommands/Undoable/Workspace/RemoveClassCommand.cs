using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public class RemoveClassCommand : IUndoableCommand
{
    private readonly AnnotationWorkspace _annotationWorkspace;
    private readonly Dictionary<ImageSpace, List<Annotation>> _affectedAnnotations = new Dictionary<ImageSpace, List<Annotation>>();
    private readonly List<IUndoableCommand> _commands = new();
    private readonly ClassData _classForDeletion;
    
    public RemoveClassCommand(AnnotationWorkspace annotationWorkspace,
        ClassData classForDeletion, 
        ClassData? classForInsertion = null)
    {
        if (!annotationWorkspace.ClassExists(classForDeletion))
        {
            throw new ConstraintException("Class to be deleted doesn't exist.", new ArgumentException(null, nameof(classForDeletion)));
        }
        
        if (annotationWorkspace.IsClassDefault(classForDeletion))
        {
            throw new ConstraintException("Deleting default class \"unassigned\" is forbidden.", new ArgumentException(null, nameof(classForDeletion)));
        }
        
        if (classForInsertion is not null && !annotationWorkspace.ClassExists(classForInsertion))
        {
            throw new ConstraintException("Class to be inserted doesn't exist.", new ArgumentException(null, nameof(classForInsertion)));
        }
        
        _annotationWorkspace = annotationWorkspace;
        var images = annotationWorkspace.Images.Where(img => 
            img.Annotations.Any(ann => ann.ClassInfo == classForDeletion));
        foreach (var img in images)
        {
            _affectedAnnotations.Add(img, img.Annotations.Where(ann => ann.ClassInfo == classForDeletion).ToList());
        }
        _classForDeletion = classForDeletion;

        if (classForInsertion is null)
        {
            foreach (var (imageSpace, annotations) in _affectedAnnotations)
            {
                IUndoableCommand cmd = new RemoveBatchAnnotationCommand(imageSpace, annotations);
                _commands.Add(cmd);
            }
        }
        else
        {
            foreach (var annotations in _affectedAnnotations.Values)
            {
                IUndoableCommand cmd = new ChangeClassBatchAnnotationCommand(annotationWorkspace, annotations, classForInsertion);
                _commands.Add(cmd);
            }
        }
    }

    public void Execute()
    {
        foreach (var cmd in _commands)
        {
            cmd.Execute();
        }
        
        _annotationWorkspace.RemoveClass(_classForDeletion);
    }

    public void Undo()
    {
        _annotationWorkspace.AddClass(_classForDeletion);
        
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }
}