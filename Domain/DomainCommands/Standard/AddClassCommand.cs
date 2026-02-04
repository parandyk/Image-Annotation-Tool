using System;
using System.Data;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Standard;

public class AddClassCommand : IPersistentCommand
{
    private readonly AnnotationWorkspace _annotationWorkspace;
    private readonly ClassData _classData;
    
    public AddClassCommand(AnnotationWorkspace annotationWorkspace, IClassDataPolicy policy, string className)
    {
        if (!policy.TrySanitizeClassName(className, out var sanitized))
        {
            throw new ConstraintException("Class name must not be empty, and it may only contain letters and digits.", 
                new ArgumentException(null, nameof(className)));
        }

        if (Equals(sanitized, "class_"))
        {
            sanitized += annotationWorkspace.ClassRunningCount;
        }
        
        if (annotationWorkspace.ClassExists(sanitized))
        {
            throw new ConstraintException("Class with specified name already exists. Class names may only contain letters and digits.", 
                new ArgumentException(null, nameof(className)));
        }
        
        _annotationWorkspace = annotationWorkspace;
        _classData = new ClassData(sanitized, policy.AssignClassColorHex(annotationWorkspace.ClassRunningCount));
        _annotationWorkspace.IncrementClassRunningCount();
    }
    
    public void Execute()
    {
        _annotationWorkspace.AddClass(_classData);
    }

    // public void Undo()
    // {
    //     _annotationWorkspace.RemoveClass(_classData);
    // }
}