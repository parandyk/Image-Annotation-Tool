using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IClassListProvider : INotifyPropertyChanged
{
    ReadOnlyObservableCollection<ClassData> Classes { get; }
    
    event Action? ActiveClassChanged;
    bool ClassExists(Guid guid);
    ClassData GetClass(Guid guid);
    ClassData GetDefaultClass();
    ClassData GetActiveClass();
}