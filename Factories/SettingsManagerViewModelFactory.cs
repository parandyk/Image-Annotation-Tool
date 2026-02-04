using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Factories;

public class SettingsManagerViewModelFactory : ISettingsManagerViewModelFactory
{
    private readonly IDialogWrapper _dialogWrapper;
    private readonly IAppMessenger _messenger;
    private readonly ICommandStack _undoRedoService;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IRenderingSettings _renderingSettings;
    private readonly IInteractionSettings _interactionSettings;
    private readonly INotificationSettings _notificationSettings;
    private readonly IAppModeSettings _appModeSettings;
    private readonly IStatisticsAggregator _statisticsAggregator;

    public SettingsManagerViewModelFactory(
        IDialogWrapper dialogWrapper,
        IAppMessenger messenger,
        ICommandStack undoRedoService,
        IWorkspaceCommandFactory commandFactory,
        IRenderingSettings renderingSettings,
        IInteractionSettings interactionSettings,
        INotificationSettings notificationSettings,
        IAppModeSettings appModeSettings,
        IStatisticsAggregator statisticsAggregator)
    {
        _dialogWrapper = dialogWrapper;
        _messenger = messenger;
        _undoRedoService = undoRedoService;
        _commandFactory = commandFactory;
        _renderingSettings = renderingSettings;
        _interactionSettings = interactionSettings;
        _notificationSettings = notificationSettings;
        _appModeSettings = appModeSettings;
        _statisticsAggregator = statisticsAggregator;
    }
        
    public SettingsManagerViewModel Create()
    {
        return new SettingsManagerViewModel(_dialogWrapper,
            _messenger,
            _undoRedoService,
            _commandFactory,
            _renderingSettings, 
            _interactionSettings, 
            _notificationSettings, 
            _appModeSettings,
            _statisticsAggregator);
    }
}