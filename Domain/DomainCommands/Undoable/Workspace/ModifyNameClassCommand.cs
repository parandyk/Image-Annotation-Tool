using System;
using System.Data;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public class ModifyNameClassCommand : IUndoableCommand
{
    private readonly ClassData _classData;
    private readonly string _newClassName;
    private readonly string _initialClassName;
    
    public ModifyNameClassCommand(AnnotationWorkspace annotationWorkspace,
        IClassDataPolicy policy,
        ClassData classData, 
        string newClassName)
    {
        if (!annotationWorkspace.ClassExists(classData))
        {
            throw new ConstraintException("Class to be modified doesn't exist.", new ArgumentException(null, nameof(classData)));
        }
        
        if (!annotationWorkspace.IsClassDefault(classData))
        {
            throw new ConstraintException("\"unassigned\" is a default class, and cannot be modified.", new ArgumentException(null, nameof(classData)));
        }

        if (!policy.IsNameValid(newClassName))
        {
            throw new ConstraintException("New class name is invalid.", new ArgumentException(null, nameof(newClassName)));
        }
        
        if (Equals(classData.Name, newClassName))
        {
            throw new ConstraintException("New class name is the same as original.", new ArgumentException(null, nameof(newClassName)));
        }
        
        if (annotationWorkspace.ClassExists(newClassName))
        {
            throw new ConstraintException("A class with this name already exists.", new ArgumentException(null, nameof(newClassName)));
        }
        
        _classData = classData;
        _initialClassName = classData.Name;
        _newClassName = newClassName;
    }
    
    public void Execute()
    {
        _classData.Rename(_newClassName);
    }

    public void Undo()
    {
        _classData.Rename(_initialClassName);
    }
}