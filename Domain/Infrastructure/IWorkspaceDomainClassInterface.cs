using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IWorkspaceDomainClassInterface : INotifyPropertyChanged
{
    event Action? ClassChanged;
    
    ReadOnlyObservableCollection<ClassData> Classes { get; }

    int ClassRunningCount { get; }

    ClassSnapshot ClassToSnapshot(ClassData classData);
    
    bool ClassExists(Guid classId);
    bool ClassExists(string className);
    bool ClassExists(ClassData classData);

    int GetClassCount();
    
    bool TryGetClassById(Guid classId, out ClassData classData);
    bool TryGetClassByName(string className, out ClassData classData);
    ClassData GetDefaultClass();
}