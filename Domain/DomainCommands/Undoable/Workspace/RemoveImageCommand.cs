using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;

public sealed class RemoveImageCommand : IUndoableCommand
{
    private readonly AnnotationWorkspace _annotationWorkspace;
    private readonly ImageSpace _imageForDeletion;
    private readonly int _index;
    
    private readonly IUndoableCommand _deleteAnnotationsCommand;
    
    public RemoveImageCommand(AnnotationWorkspace annotationWorkspace, ImageSpace imageForDeletion)
    {
        if (!annotationWorkspace.ImageExists(imageForDeletion))
        {
            throw new ConstraintException("Image to be deleted doesn't exist.", new ArgumentException(null, nameof(imageForDeletion)));
        }
        
        _annotationWorkspace = annotationWorkspace;
        _imageForDeletion = imageForDeletion;
        
        _index = annotationWorkspace.Images.IndexOf(_imageForDeletion);

        var annotations = imageForDeletion.Annotations.ToList();
        
        _deleteAnnotationsCommand = new RemoveBatchAnnotationCommand(imageForDeletion, annotations);
    }
    
    public void Execute()
    {
        _deleteAnnotationsCommand.Execute();
        _annotationWorkspace.RemoveImage(_imageForDeletion);
    }

    public void Undo()
    {
        _deleteAnnotationsCommand.Undo();
        _annotationWorkspace.InsertImage(_index, _imageForDeletion);
    }
}