using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.DataTransferObjects.General;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ExportableObjects;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Domain.Infrastructure.SettingsStore;
using ImageAnnotationTool.Domain.Infrastructure.UseCases;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Factories;
using ImageAnnotationTool.Interfaces;

namespace ImageAnnotationTool.ViewModels;

public partial class WorkspaceManagerViewModel : ObservableObject
{
    private readonly INotificationSettings _notificationSettings;
    private readonly IUseCaseProvider _useCaseProvider;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly ICommandStack _undoRedoService;
    private readonly IFileAccessProvider _filesProvider;
    private readonly IAppMessenger _messenger;
    private readonly IWorkspaceDomainInterface _domain;
    
    public WorkspaceManagerViewModel(INotificationSettings notificationSettings,
        IUseCaseProvider useCaseProvider,
        IImageManagerViewModelFactory imVmFactory,
        IClassManagerViewModelFactory cmVmFactory,
        IWorkspaceCommandFactory commandFactory,
        ICommandStack undoRedoService,
        IFileAccessProvider filesProvider,
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger,
        IWorkspaceDomainInterface domain)
    {
        _notificationSettings = notificationSettings;
        _useCaseProvider = useCaseProvider;
        _filesProvider = filesProvider;
        _commandFactory = commandFactory;
        _dialogWrapper = dialogWrapper;
        _messenger = messenger;
        _domain = domain;
        
        _undoRedoService = undoRedoService;
        _undoRedoService.StacksChanged += ChangeInStacks;

        ClassManagerVM = cmVmFactory.Create();
        ImageManagerVM = imVmFactory.Create();
        ImageManagerVM.PropertyChanged += OnImageManagerVMChanged;
        ((INotifyCollectionChanged)ClassManagerVM.ClassList).CollectionChanged += OnClassManagerVMClassListChanged;
    }

    private void OnClassManagerVMClassListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null || e.OldItems != null)
        {
            OnPropertyChanged(nameof(ClassesPresent));
        }
    }

    [ObservableProperty] 
    private string? _fileText;
    
    public ClassManagerViewModel ClassManagerVM { get; }

    public ImageManagerViewModel ImageManagerVM { get; }
    
    public bool UndoStackNotEmpty => _undoRedoService.UndoStackNotEmpty;
    
    public bool RedoStackNotEmpty => _undoRedoService.RedoStackNotEmpty;
    
    public bool ImageOpen => ImageManagerVM.SelectedImageViewModel is not null;

    public bool ClassesPresent => ClassManagerVM.ClassList.Any(m => !Equals(m, ClassManagerVM.DefaultClass));
    
    private void OnImageManagerVMChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageManagerViewModel.SelectedImageViewModel))
        {
            OnPropertyChanged(nameof(ImageOpen));
        }
    }
    
    private void ChangeInStacks()
    {
        OnPropertyChanged(nameof(UndoStackNotEmpty));
        OnPropertyChanged(nameof(RedoStackNotEmpty));
    }
    
    [RelayCommand]
    private void AbortOperation()
    {
        _messenger.SendAbortOperationRequest();
    }
    
    [RelayCommand]
    private void UndoOperation()
    {
        if (!UndoStackNotEmpty)
            return;
        
        try
        { 
            _undoRedoService.Undo();
        }
        catch (Exception ex)
        {
            _messenger.SendErrorOccurredNotification(ex.Message);
        }
    }

    [RelayCommand]
    private void RedoOperation()
    {
        if (!RedoStackNotEmpty)
            return;
        
        try
        {
            _undoRedoService.Redo();
        }
        catch (Exception ex)
        {
            _messenger.SendErrorOccurredNotification(ex.Message);
        }
    }
    
    [RelayCommand] //TODO
    private async Task OpenFolder(CancellationToken token)
    {
        throw new NotImplementedException();
    }
    
    [RelayCommand]
    private async Task OpenImageFile(CancellationToken token)
    {
        try
        {
            if (_filesProvider is null) 
                throw new NullReferenceException("Missing file service instance.");

            var imageList = await _filesProvider.OpenImageFileAsync();
            if (imageList is null) return;
            
            ImageManagerVM.AddImageCommand.Execute(imageList);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand] //TODO
    private async Task OpenClassFile(CancellationToken token)
    {
        try
        { 
            if (_filesProvider is null) 
                throw new NullReferenceException("Missing file service instance.");

            var classList = await _filesProvider.OpenClassFileAsync();
            
            if (classList is null) return;

            foreach (var className in classList)
            {
                ClassManagerVM.CreateNewClassCommand.Execute(className);
            }

        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand(CanExecute = nameof(ImageOpen))] //TODO
    private async Task OpenAnnotationFile(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task SaveLocalAnnotations(AnnotationFormat format) //TODO
    {
        try
        {
            await SaveAnnotations(format, false);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    
    [RelayCommand] //TODO
    private async Task SaveGlobalAnnotations(AnnotationFormat format)
    {
        try
        {
            await SaveAnnotations(format, true);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }

    private async Task SaveAnnotations(AnnotationFormat format, bool global = true)
    {
        try
        {
            if (_filesProvider is null) 
                throw new NullReferenceException("Missing file service instance.");
            
            var path = await _filesProvider.OpenOutputFolderAsync();
            if (path is null)
            {
                _messenger.SendErrorOccurredNotification("Annotations export aborted.");
                return;
            }

            bool includeFallback = false;
            
            if (!_notificationSettings.SuppressUnassignedExportWarningDialog)
            {
                ImageSpace? imageSpace = null;
                
                if (!global)
                {
                    var imageVm = ImageManagerVM.SelectedImageViewModel;

                    if (imageVm is null)
                        return;
                    
                    imageSpace = imageVm.ImageSpace;
                }
                
                var vm = _dialogWrapper.CreateUnassignedWarningDialog(imageSpace);
                var result = await _dialogWrapper.ShowDialogAsync(vm);

                if (result is false)
                {
                    _messenger.SendErrorOccurredNotification("Annotations export aborted.");
                    return;
                }
                
                if (vm.DontAskAgain)
                    _notificationSettings.ChangeSuppressionUnassignedExportWarningDialog(true);

                includeFallback = vm.IncludeUnassigned;
            }
            
            var exportContext = CreateAnnotationExportContext(path, format, global, includeFallback);

            if (exportContext is null)
            {
                throw new NullReferenceException("No valid objects for export."); //TODO
            }
            
            _useCaseProvider.ExportAnnotations(exportContext);
            _messenger.SendErrorOccurredNotification("Annotations exported successfully.");
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }

    private List<ClassSnapshot>? CreateClassSnapshots(bool global = true, bool includeFallback = false)
    {
        List<ClassSnapshot> classSnapshots;
        
        if (global)
        {
            classSnapshots = _domain.Classes
                .Where(c => !Equals(c, ClassManagerVM.DefaultClass))
                .Select(c => _domain.ClassToSnapshot(c))
                .OrderBy(c => c.Name)
                .ToList();
            
            if (includeFallback)
                classSnapshots.Add(_domain.ClassToSnapshot(ClassManagerVM.DefaultClass));
        }
        else
        {
            if (ImageManagerVM.SelectedImageViewModel is null)
                return null;
            
            var imageSpace = ImageManagerVM.SelectedImageViewModel.ImageSpace;
            
            classSnapshots = imageSpace.Annotations
                .Where(a => !Equals(a.ClassInfo, ClassManagerVM.DefaultClass))
                .Select(a => _domain.ClassToSnapshot(a.ClassInfo))
                .Distinct()
                .OrderBy(c => c.Name)
                .ToList();
            
            if (includeFallback && imageSpace.Annotations.Any(a => Equals(a.ClassInfo, ClassManagerVM.DefaultClass)))
                classSnapshots.Add(_domain.ClassToSnapshot(ClassManagerVM.DefaultClass));
        }
        
        return classSnapshots;
    }
    
    private List<ImageSnapshot>? CreateImageSnapshots(bool global = true, bool includeFallback = false)
    {
        List<ImageSnapshot> imageSnapshots = new();
        
        if (global)
        {
            imageSnapshots = _domain.Images.
                Select(img => _domain.ImageToSnapshot(img, includeFallback))
                .ToList();
        }
        else
        {
            if (ImageManagerVM.SelectedImageViewModel is null)
                return null;
            
            var imageSpace = ImageManagerVM.SelectedImageViewModel.ImageSpace;
            var snapshot = _domain.ImageToSnapshot(imageSpace, includeFallback);
            imageSnapshots.Add(snapshot);
        }
        
        return imageSnapshots;
    }

    private ClassExportContext? CreateClassExportContext(string path, ClassFormat format, bool global = true,
        bool includeFallback = false)
    {
        var classes = CreateClassSnapshots(global, includeFallback);

        if (classes is null || 
            classes.Count == 0)
            return null;
        
        var exportableClasses = classes
            .Select(c => new ExportableClass(c))
            .ToList();
        
        return new ClassExportContext(path, format, exportableClasses);
    }
    
    private AnnotationExportContext? CreateAnnotationExportContext(string path,
        AnnotationFormat format, 
        bool global = true, 
        bool includeFallback = false)
    {
        var classes = CreateClassSnapshots(global, includeFallback);
        var images = CreateImageSnapshots(global, includeFallback);
        
        if (classes is null || 
            images is null ||
            classes.Count == 0 ||
            images.Count == 0)
            return null;
        
        var exportableClasses = classes
            .Select(c => new ExportableClass(c))
            .ToList();
        var exportableImages = images
            .Select(i => new ExportableImage(i))
            .ToList();
        
        return new AnnotationExportContext(path, format, exportableImages, exportableClasses);
    }
    
    [RelayCommand]
    private async Task SaveClasses(ClassFormat format)
    {
        try
        {
            if (_filesProvider is null) 
                throw new NullReferenceException("Missing file service instance.");
            
            var path = await _filesProvider.OpenOutputFolderAsync();
            if (path is null)
            {
                _messenger.SendErrorOccurredNotification("Classes export aborted.");
                return;
            }
            
            var exportContext = CreateClassExportContext(path, format, true);

            if (exportContext is null)
            {
                throw new NullReferenceException("No valid objects for export.");
            }
            
            _useCaseProvider.ExportClasses(exportContext);
            _messenger.SendErrorOccurredNotification("Classes exported successfully.");
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
}