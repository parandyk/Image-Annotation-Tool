using System;
using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IClassStore : IClassProvider, IClassListProvider
{
    void AddClass(string className);
    void RemoveClass(Guid classGuid, Guid? replacementGuid = null);
    void RenameClass(Guid guid, string newClassName);
    
    void AddBatchClass(List<string> classNames);
    void RemoveBatchClass(Dictionary<Guid, Guid?> deleteInsertClassDict);
    void RenameBatchClass(Dictionary<string, Guid> namesClassesDict);
    
    void ChangeVisibilityClass(Guid classGuid, bool? visibility);
    void ChangeBatchVisibilityClass(List<Guid> classGuids, bool? visibility);
}