using System;
using System.Data;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DomainCommands.Standard;

public class AddImageCommand : IPersistentCommand
{
    private readonly AnnotationWorkspace _annotationWorkspace;
    private readonly ImageSpace _imageSpace;
    
    public AddImageCommand(AnnotationWorkspace annotationWorkspace, ImageSource imageSource, ImageMetadata imageMetadata)
    {
        _imageSpace = new ImageSpace(imageSource, imageMetadata, annotationWorkspace.ImageRunningCount);
        
        if (annotationWorkspace.ImageExists(_imageSpace))
        {
            throw new ConstraintException("Image to be added already exists.");
        }

        if (annotationWorkspace.ImageExists(imageSource.ImagePath))
        {
            throw new ConstraintException("An image with the same path already exists.", new ArgumentException(null, nameof(imageSource.ImagePath)));
        }
        
        if (annotationWorkspace.ImageWithNameExists(imageSource.ImageName))
        {
            throw new ConstraintException("An image with the same name already exists.", new ArgumentException(null, nameof(imageSource.ImageName)));
        }
        
        _annotationWorkspace = annotationWorkspace;
        annotationWorkspace.IncrementImageRunningCount();
    }
    
    public void Execute()
    {
        _annotationWorkspace.AddImage(_imageSpace);
    }

    // public void Undo()
    // {
    //     _annotationWorkspace.RemoveImage(_imageSpace);
    // }
}