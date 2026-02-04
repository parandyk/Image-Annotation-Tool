using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Enums;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Models;


namespace ImageAnnotationTool.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IFileAccessProvider _filesProvider;
    private readonly IAppMessenger _messenger;
    private readonly ICommandStack _undoRedoService;

    public MainViewModel(IFileAccessProvider filesProvider,
        IImageManagerViewModelFactory imVmFactory,
        IClassManagerViewModelFactory cmVmFactory,
        ISettingsManagerViewModelFactory smVmFactory,
        IAppMessenger messenger,
        ICommandStack undoRedoService)
    {
        _filesProvider = filesProvider;
        
        _messenger = messenger;
        _messenger.Register<ErrorOccurredMessage>(this, OnErrorOccurredNotification);
        
        _undoRedoService = undoRedoService;
        _undoRedoService.StacksChanged += ChangeInStacks;

        ClassManagerVM = cmVmFactory.Create();
        ImageManagerVM = imVmFactory.Create();
        SettingsManagerVM = smVmFactory.Create();

        ImageManagerVM.PropertyChanged += OnImageManagerVMChanged;
    }

    private void ChangeInStacks()
    {
        OnPropertyChanged(nameof(UndoStackNotEmpty));
        OnPropertyChanged(nameof(RedoStackNotEmpty));
    }

    private void OnErrorOccurredNotification(object recipient, ErrorOccurredMessage message)
    {
        ErrorMessagesNotifications?.Clear();
        
        if (message.error is not null)
            ErrorMessagesNotifications?.Add(message.error);
    }
    
    public ClassManagerViewModel ClassManagerVM { get; }

    public ImageManagerViewModel ImageManagerVM { get; }
    
    public SettingsManagerViewModel SettingsManagerVM { get; }

    [ObservableProperty] 
    private string? _annotationFormat;
    
    [ObservableProperty] 
    private string? _fileText;
    
    public bool UndoStackNotEmpty => _undoRedoService.UndoStackNotEmpty;
    
    public bool RedoStackNotEmpty => _undoRedoService.RedoStackNotEmpty;
    
    public bool ImageOpen => ImageManagerVM.SelectedImageViewModel is not null;

    public bool ClassesPresent => ClassManagerVM.ClassList.Any(m => !Equals(m.Name, "unassigned"));

    [ObservableProperty]
    private bool _hideUnannotated = false;

    [ObservableProperty]
    private bool _hideAnnotated = false;

    private void OnImageManagerVMChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImageManagerViewModel.SelectedImageViewModel))
        {
            OnPropertyChanged(nameof(ImageOpen));
        }
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
            ErrorMessagesNotifications?.Clear();
            ErrorMessagesNotifications?.Add(ex.Message);
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
            ErrorMessagesNotifications?.Clear();
            ErrorMessagesNotifications?.Add(ex.Message);
        }
    }
    
    /// Asynchronous methods

    [RelayCommand]
    private async Task OpenFolder(CancellationToken token)
    {
        throw new NotImplementedException();
    }
    
    [RelayCommand]
    private async Task OpenImageFile(CancellationToken token)
    {
        ErrorMessagesNotifications?.Clear();
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
            ErrorMessagesNotifications?.Add(e.Message);
        }
    }
    
    [RelayCommand] //TODO
    private async Task OpenClassFile(CancellationToken token)
    {
        ErrorMessagesNotifications?.Clear();
        try
        { 
            if (_filesProvider is null) throw new NullReferenceException("Missing File Service instance.");

            var classList = await _filesProvider.OpenClassFileAsync();
            
            if (classList is null) return;

            foreach (var className in classList)
            {
                ClassManagerVM.CreateNewClassCommand.Execute(className);
            }

        }
        catch (Exception e)
        {
            ErrorMessagesNotifications?.Add(e.Message);
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
        throw new  NotImplementedException();
        
        // ErrorMessages?.Clear();
        // try
        // {
        //     if (_filesProvider is null) throw new NullReferenceException("Missing File Service instance.");
        //
        //     var file = await _filesProvider.SaveAnnotationFileAsync();
        //     if (file is null) return;
        //     
        //     var stream = new MemoryStream(Encoding.Default.GetBytes((string)FileText));
        //     await using var writeStream = await file.OpenWriteAsync();
        //     await stream.CopyToAsync(writeStream);
        //
        //     // BBoxesSinceLastSave = 0;
        //
        // }
        // catch (Exception e)
        // {
        //     ErrorMessages?.Add(e.Message);
        // }
    }
    
    [RelayCommand] //TODO
    private async Task SaveClassesToFile()
    {
        throw new  NotImplementedException();
        
        ErrorMessagesNotifications?.Clear();
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
            ErrorMessagesNotifications?.Add(e.Message);
        }
    }
}