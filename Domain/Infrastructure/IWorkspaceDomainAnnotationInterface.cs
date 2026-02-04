using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IWorkspaceDomainAnnotationInterface : INotifyPropertyChanged//TODO: KEEP?
{
    ReadOnlyObservableCollection<Annotation> AllAnnotations { get; }
    
    event Action? AnnotationChanged;
    
    bool AnnotationExists(Annotation annotation);
    bool AnnotationExists(Guid annotationId);
    
    int GetAnnotationCount();
    
    bool TryGetAnnotationById(Guid annotationId, out Annotation annotation);
}