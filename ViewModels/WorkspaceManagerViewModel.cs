using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Interfaces;

namespace ImageAnnotationTool.ViewModels;

public partial class WorkspaceManagerViewModel : ObservableObject
{
    private readonly ExportAnnotationsUseCase _exportAnnotationsUseCase;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly ICommandStack _undoRedoService;
    private readonly IFileAccessProvider _filesProvider;
    private readonly IAppMessenger _messenger;
    
    public WorkspaceManagerViewModel(ExportAnnotationsUseCase exportAnnotationsUseCase,
        IImageManagerViewModelFactory imVmFactory,
        IClassManagerViewModelFactory cmVmFactory,
        IWorkspaceCommandFactory commandFactory,
        ICommandStack undoRedoService,
        IFileAccessProvider filesProvider,
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger)
    {
        _exportAnnotationsUseCase = exportAnnotationsUseCase;
        _filesProvider = filesProvider;
        _commandFactory = commandFactory;
        _dialogWrapper = dialogWrapper;
        _messenger = messenger;
        
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
    
    [RelayCommand] //TODO
    private async Task SaveAnnotationsToFile()
    {
        try
        {
            
            
            // if (_filesProvider is null) 
            //     throw new NullReferenceException("Missing file service instance.");
            //
            //
            //
            // var file = await _filesProvider.SaveAnnotationFileAsync();
            // if (file is null) 
            //     return;
            //
            // var stream = new MemoryStream(Encoding.Default.GetBytes((string)FileText));
            // await using var writeStream = await file.OpenWriteAsync();
            // await stream.CopyToAsync(writeStream);
        }
        catch (Exception e)
        {
            _messenger.SendErrorOccurredNotification(e.Message);
        }
    }

    private void ExportAnnotations()
    {
        var snapshots = ImageManagerVM.ImageViewModelList.Select(vm => vm.ClassList);
    }
    
    [RelayCommand] //TODO
    private async Task SaveClassesToFile()
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