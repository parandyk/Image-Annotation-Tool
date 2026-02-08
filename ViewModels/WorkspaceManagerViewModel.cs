using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.ExportableObjects;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.Infrastructure.ExportContexts;
using ImageAnnotationTool.Domain.Infrastructure.UseCases;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Factories;
using ImageAnnotationTool.Interfaces;

namespace ImageAnnotationTool.ViewModels;

public partial class WorkspaceManagerViewModel : ObservableObject
{
    private readonly IUseCaseProvider _useCaseProvider;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly ICommandStack _undoRedoService;
    private readonly IFileAccessProvider _filesProvider;
    private readonly IAppMessenger _messenger;
    private readonly IWorkspaceDomainInterface _domain;
    
    public WorkspaceManagerViewModel(IUseCaseProvider useCaseProvider,
        IImageManagerViewModelFactory imVmFactory,
        IClassManagerViewModelFactory cmVmFactory,
        IWorkspaceCommandFactory commandFactory,
        ICommandStack undoRedoService,
        IFileAccessProvider filesProvider,
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger,
        IWorkspaceDomainInterface domain)
    {
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
            _messenger.SendErrorOccurredNotification("Annotations exported successfully.");
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
            _messenger.SendErrorOccurredNotification("Annotations exported successfully.");
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
                return;
            
            var exportContext = CreateExportContext(path, format);

            if (exportContext is null)
            {
                throw new NullReferenceException("No valid objects for export."); //TODO
            }
            
            _useCaseProvider.ExportAnnotations(exportContext);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
    

    private List<ClassSnapshot>? CreateClassSnapshots(bool global = true)
    {
        List<ClassSnapshot> classSnapshots;
        
        if (global)
        {
            classSnapshots = _domain.Classes.Where(c => !Equals(c, ClassManagerVM.DefaultClass))
                .Select(c => _domain.ClassToSnapshot(c))
                .OrderBy(c => c.Name)
                .ToList();
        }
        else
        {
            if (ImageManagerVM.SelectedImageViewModel is null)
                return null;
            
            var imageSpace = ImageManagerVM.SelectedImageViewModel.ImageSpace;
            
            classSnapshots = imageSpace.Annotations.Where(a => !Equals(a.ClassInfo, ClassManagerVM.DefaultClass))
                .Select(a => _domain.ClassToSnapshot(a.ClassInfo))
                .Distinct()
                .OrderBy(c => c.Name)
                .ToList();
        }
        
        return classSnapshots;
    }
    
    private List<ImageSnapshot>? CreateImageSnapshots(bool global = true)
    {
        List<ImageSnapshot> imageSnapshots = new();
        
        if (global)
        {
            imageSnapshots = _domain.Images.
                Select(img => _domain.ImageToSnapshot(img))
                .ToList();
        }
        else
        {
            if (ImageManagerVM.SelectedImageViewModel is null)
                return null;
            
            var imageSpace = ImageManagerVM.SelectedImageViewModel.ImageSpace;
            var snapshot = _domain.ImageToSnapshot(imageSpace);
            imageSnapshots.Add(snapshot);
        }
        
        return imageSnapshots;
    }

    private AnnotationExportContext? CreateExportContext(string path,
        AnnotationFormat format, 
        bool global = true)
    {
        
        var classes = CreateClassSnapshots(global);
        var images = CreateImageSnapshots(global);

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
    
    [RelayCommand] //TODO
    private async Task SaveClassesToFile(ClassFormat format)
    {
        throw new  NotImplementedException();
        
        try
        {
            if (_filesProvider is null) throw new NullReferenceException("Missing File Service instance.");

            var file = await _filesProvider.SaveClassesFileAsync();
            if (file is null) return;
            
            var stream = new MemoryStream(Encoding.Default.GetBytes((string)FileText));
            await using var writeStream = await file.OpenWriteAsync();
            await stream.CopyToAsync(writeStream);

            // BBoxesSinceLastSave = 0;

        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }
}