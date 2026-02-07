using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Factories;

public class WorkspaceManagerViewModelFactory : IWorkspaceManagerViewModelFactory
{
    private readonly ExportAnnotationsUseCase _exportAnnotationsUseCase;
    private readonly IImageManagerViewModelFactory _imVmFactory;
    private readonly IClassManagerViewModelFactory _cmVmFactory;
    private readonly IWorkspaceCommandFactory _commandFactory;
    // private readonly IWorkspaceDomainInterface _domain;
    private readonly IFileAccessProvider _fileAccessProvider;
    private readonly ICommandStack _undoRedoService;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly IAppMessenger _messenger;

    public WorkspaceManagerViewModelFactory(ExportAnnotationsUseCase exportAnnotationsUseCase,
        IImageManagerViewModelFactory imVmFactory,
        IClassManagerViewModelFactory cmVmFactory,
        IWorkspaceCommandFactory commandFactory,
        // IWorkspaceDomainInterface domain,
        ICommandStack undoRedoService,
        IFileAccessProvider fileAccessProvider,
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger)
    {
        _exportAnnotationsUseCase = exportAnnotationsUseCase;
        _imVmFactory = imVmFactory;
        _cmVmFactory = cmVmFactory;
        _commandFactory = commandFactory;
        // _domain = domain;
        _fileAccessProvider = fileAccessProvider;
        _dialogWrapper = dialogWrapper;
        _messenger = messenger;
        _undoRedoService = undoRedoService;
    }
    
    public WorkspaceManagerViewModel Create()
    {
        return new WorkspaceManagerViewModel(
            _exportAnnotationsUseCase,
            _imVmFactory,
            _cmVmFactory,
            _commandFactory,
            _undoRedoService,
            _fileAccessProvider,
            _dialogWrapper,
            _messenger);
    }
}