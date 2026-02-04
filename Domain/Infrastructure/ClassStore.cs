using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Classes.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Domain.Infrastructure;

public sealed class ClassStore : ObservableObject, IClassStore
{
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IWorkspaceDomainClassInterface _workspace;
    private readonly ICommandStack _undoRedoService;
    
    public ClassStore(IWorkspaceDomainClassInterface workspace, 
        ICommandStack undoRedoService, 
        IWorkspaceCommandFactory commandFactory)
    {
        _workspace = workspace;
        _undoRedoService = undoRedoService;
        _commandFactory = commandFactory;
        
        _activeClass = workspace.GetDefaultClass();
        
        ((INotifyCollectionChanged)_workspace.Classes).CollectionChanged += OnDomainChanged;
    }

    private void OnDomainChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ActiveClass));
        OnPropertyChanged(nameof(Classes));
    }

    public ReadOnlyObservableCollection<ClassData> Classes => _workspace.Classes;

    private ClassData _activeClass;
    
    public ClassData ActiveClass
    {
        get => _activeClass;
        set
        {
            if (!_workspace.ClassExists(value)) return;
            
            _activeClass = value;
            OnPropertyChanged(nameof(ActiveClass));
        }
    }
    
    public void AddClass(string className)
    {
        _undoRedoService.Execute(_commandFactory.AddClass(className));
    }

    public void RemoveClass(Guid guid, Guid? replacementGuid = null)
    {
        var classData = GetClass(guid);
        ClassData? replacementClassData = null;
        
        if (replacementGuid != null)
        {
            replacementClassData = GetClass(replacementGuid.Value);
        }
        
        _undoRedoService.Execute(_commandFactory.RemoveClass(classData, replacementClassData));
    }

    public void RenameClass(Guid guid, string newClassName)
    {
        var classData = GetClass(guid);
        _undoRedoService.Execute(_commandFactory.ModifyNameClass(classData, newClassName));
    }

    public bool ClassExists(Guid guid)
    {
        return _workspace.ClassExists(guid);
    }

    public void AddBatchClass(List<string> classNames)
    {
        _undoRedoService.Execute(_commandFactory.AddBatchClass(classNames));
    }

    public void RemoveBatchClass(Dictionary<Guid, Guid?> deleteInsertClassDict)
    {
        var mapDictionary = new Dictionary<ClassData, ClassData?>();
        
        foreach (var (deletionGuid, replacementGuid) in deleteInsertClassDict)
        {
            var deletionClassData = GetClass(deletionGuid);
            ClassData? replacementClassData = null;
            
            if (replacementGuid != null)
                replacementClassData = GetClass(replacementGuid.Value);
            
            mapDictionary.Add(deletionClassData, replacementClassData);
        }
        
        _undoRedoService.Execute(_commandFactory.RemoveBatchClass(mapDictionary));
    }

    public void RenameBatchClass(Dictionary<string, Guid> namesClassesDict)
    {
        var mapDictionary = new Dictionary<string, ClassData>();
        
        foreach (var (className, classGuid) in namesClassesDict)
        {
            var classData = GetClass(classGuid);
            
            mapDictionary.Add(className, classData);
        }
        
        _undoRedoService.Execute(_commandFactory.ModifyNameBatchClass(mapDictionary));
    }

    public void ChangeVisibilityClass(Guid classGuid, bool? visibility)
    {
        var classData = GetClass(classGuid);
        _undoRedoService.Execute(new ChangeVisibilityClassCommand(classData, visibility));
    }

    public void ChangeBatchVisibilityClass(List<Guid> classGuids, bool? visibility)
    {
        var classList = classGuids.Select(x => GetClass(x)).ToList();
        _undoRedoService.Execute(new ChangeVisibilityBatchClassCommand(classList, visibility));
    }

    public ClassData GetClass(Guid guid)
    {
        return _workspace.Classes.First(c => c.Id == guid);
    }

    public ClassData GetDefaultClass()
    {
        return _workspace.GetDefaultClass();
    }

    public ClassData GetActiveClass()
    {
        return _activeClass;
    }
}