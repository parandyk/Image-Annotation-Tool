using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IImageDomainInterface : INotifyPropertyChanged
{
    ReadOnlyObservableCollection<Annotation> Annotations { get; }
    
    public event Action? AnnotationChanged;
    
    int AnnotationRunningCount { get; }
    
    bool AnnotationExists(Annotation annotation);
    bool AnnotationExists(Guid annotationId);
    bool TryGetAnnotationById(Guid annotationId, out Annotation annotation);
    
    int GetAnnotationCount();
}