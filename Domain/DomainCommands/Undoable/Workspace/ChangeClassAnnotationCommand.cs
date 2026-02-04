using System;
using System.Data;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public sealed class ChangeClassAnnotationCommand : IUndoableCommand
{
    private readonly Annotation _annotation;
    private readonly ClassData _initialClass;
    private readonly ClassData? _newClass;
    
    public ChangeClassAnnotationCommand(AnnotationWorkspace annotationWorkspace, Annotation annotation, ClassData? newClass)
    {
        if (newClass != null && !annotationWorkspace.ClassExists(newClass))
        {
            throw new ConstraintException("Class to be inserted doesn't exist.", new ArgumentException(null, nameof(newClass)));
        }
        
        _annotation = annotation;
        _initialClass = annotation.ClassInfo;
        _newClass = newClass;
    }
    
    public void Execute()
    {
        _annotation.ChangeClass(_newClass);
    }

    public void Undo()
    {
        _annotation.ChangeClass(_initialClass);
    }
}