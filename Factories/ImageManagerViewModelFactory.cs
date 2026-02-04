using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Interfaces;
using IClassListProvider = ImageAnnotationTool.Domain.Infrastructure.IClassListProvider;
using IImageViewModelFactory = ImageAnnotationTool.Domain.Infrastructure.IImageViewModelFactory;
using ImageManagerViewModel = ImageAnnotationTool.ViewModels.ImageManagerViewModel;

namespace ImageAnnotationTool.Factories;

public class ImageManagerViewModelFactory : IImageManagerViewModelFactory
{
    private readonly IWorkspaceDomainImageInterface _workspace;
    private readonly INotificationSettings _notificationSettings;
    private readonly IImageViewModelFactory _imgFactory;
    private readonly IClassListProvider _classListProvider;
    private readonly ICommandStack _undoRedoService;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IStatisticsAggregator _statisticsAggregator;
    private readonly IAppMessenger _messenger;
    private readonly IDialogWrapper _dialogWrapper;
    
    public ImageManagerViewModelFactory(IWorkspaceDomainImageInterface workspace,
        INotificationSettings notificationSettings,
        IImageViewModelFactory imgFactory,
        IClassListProvider classListProvider,
        ICommandStack undoRedoService,
        IWorkspaceCommandFactory commandFactory,
        IStatisticsAggregator statisticsAggregator,
        IAppMessenger messenger,
        IDialogWrapper dialogWrapper)
    {
        _workspace = workspace;
        _notificationSettings = notificationSettings;
        _imgFactory = imgFactory;
        _classListProvider = classListProvider;
        _undoRedoService = undoRedoService;
        _commandFactory = commandFactory;
        _statisticsAggregator = statisticsAggregator;
        _messenger = messenger;
        _dialogWrapper = dialogWrapper;
    }

    public ImageManagerViewModel Create()
    {
        return new ImageManagerViewModel(_workspace,
            _imgFactory,
            _classListProvider,
            _notificationSettings,
            _undoRedoService,
            _commandFactory,
            _statisticsAggregator,
            _messenger,
            _dialogWrapper);
    }
}