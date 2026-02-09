using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.Infrastructure.SettingsStore;
using ImageAnnotationTool.Domain.Infrastructure.UseCases;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Factories;

public class WorkspaceManagerViewModelFactory : IWorkspaceManagerViewModelFactory
{
    private readonly INotificationSettings _notificationSettings;
    private readonly IUseCaseProvider _useCaseProvider;
    private readonly IImageManagerViewModelFactory _imVmFactory;
    private readonly IClassManagerViewModelFactory _cmVmFactory;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IFileAccessProvider _fileAccessProvider;
    private readonly ICommandStack _undoRedoService;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly IAppMessenger _messenger;
    private readonly IWorkspaceDomainInterface _domain;

    public WorkspaceManagerViewModelFactory(INotificationSettings notificationSettings,
        IUseCaseProvider useCaseProvider,
        IImageManagerViewModelFactory imVmFactory,
        IClassManagerViewModelFactory cmVmFactory,
        IWorkspaceCommandFactory commandFactory,
        ICommandStack undoRedoService,
        IFileAccessProvider fileAccessProvider,
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger, 
        IWorkspaceDomainInterface domain)
    {
        _notificationSettings = notificationSettings;
        _useCaseProvider = useCaseProvider;
        _imVmFactory = imVmFactory;
        _cmVmFactory = cmVmFactory;
        _commandFactory = commandFactory;
        _fileAccessProvider = fileAccessProvider;
        _dialogWrapper = dialogWrapper;
        _messenger = messenger;
        _undoRedoService = undoRedoService;
        _domain = domain;
    }
    
    public WorkspaceManagerViewModel Create()
    {
        return new WorkspaceManagerViewModel(_notificationSettings,
            _useCaseProvider,
            _imVmFactory,
            _cmVmFactory,
            _commandFactory,
            _undoRedoService,
            _fileAccessProvider,
            _dialogWrapper,
            _messenger,
            _domain);
    }
}