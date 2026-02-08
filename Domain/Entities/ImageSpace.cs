using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.DomainCommands;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.Entities;

public sealed class ImageSpace : ObservableObject, IImageDomainInterface
{
    public ImageSpace(ImageSource imageSource, ImageMetadata imageMetadata, int exportId)
    {
        Source = imageSource;
        Metadata = imageMetadata;
        ExportId = exportId;
    }
    
    public event Action? AnnotationChanged;
    
    public Guid Id { get; } = Guid.NewGuid();
    
    public int ExportId { get; }

    public int AnnotationRunningCount { get; private set; } = 0;

    private readonly ObservableCollection<Annotation> _annotations = new();
    public ReadOnlyObservableCollection<Annotation> Annotations => 
        new ReadOnlyObservableCollection<Annotation>(_annotations);

    public ImageSource Source { get; }
    
    public ImageMetadata Metadata { get; }
    
    internal void IncrementAnnotationRunningCount()
    {
        AnnotationRunningCount++;
    }
    
    internal void AddAnnotation(Annotation annotation)
    {
        annotation.AnnotationChanged += OnAnnotationChanged;
        _annotations.Add(annotation);
        AnnotationChanged?.Invoke();
    }

    internal void RemoveAnnotation(Annotation annotation)
    {
        annotation.AnnotationChanged -= OnAnnotationChanged;
        _annotations.Remove(annotation);
        AnnotationChanged?.Invoke();
    }
    
    private void OnAnnotationChanged()
    {
        AnnotationChanged?.Invoke();
    }

    internal void MoveAnnotation(Annotation annotation, BoundingBox newBounds)
    {
        MutateAnnotationBounds(annotation, newBounds);
    }
    
    internal void ResizeAnnotation(Annotation annotation, BoundingBox newBounds)
    {
        MutateAnnotationBounds(annotation, newBounds);
    }

    private void MutateAnnotationBounds(Annotation annotation, BoundingBox newBounds)
    {
        if (!BoundsFitImage(newBounds))
        {
            throw new ConstraintException("The annotation must fit within the image.", new ArgumentOutOfRangeException("newBounds"));
        }
        
        annotation.SetBounds(newBounds);
    }

    private bool BoundsFitImage(BoundingBox newBounds)
    {
        return newBounds.X1 <= Metadata.ImagePixelWidth && 
               newBounds.X2 <= Metadata.ImagePixelWidth && 
               newBounds.Y1 <= Metadata.ImagePixelHeight && 
               newBounds.Y2 <= Metadata.ImagePixelHeight;
    }
    
    public bool AnnotationExists(Guid annotationId)
    {
        return _annotations.Any(annotation => annotation.Id == annotationId);
    }

    public bool AnnotationExists(Annotation annotation)
    {
        return _annotations.Contains(annotation);
    }

    internal Annotation GetAnnotation(Guid annotationId)
    {
        if (!AnnotationExists(annotationId))
        {
            throw new  ConstraintException("Annotation not found.", new ArgumentOutOfRangeException("annotationId"));
        }
        
        return _annotations.First(annotation => annotation.Id == annotationId);
    }
    
    public bool TryGetAnnotationById(Guid annotationId, out Annotation annotation)
    {
        annotation = _annotations.FirstOrDefault(annotation => annotation.Id == annotationId);
        return annotation != null;
    }

    public int GetAnnotationCount()
    {
        return _annotations.Count;
    }
}