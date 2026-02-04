using HanumanInstitute.MvvmDialogs;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Interfaces;
using IAnnotationViewModelFactory = ImageAnnotationTool.Domain.Infrastructure.IAnnotationViewModelFactory;
using IClassListProvider = ImageAnnotationTool.Domain.Infrastructure.IClassListProvider;
using IImageViewModelFactory = ImageAnnotationTool.Domain.Infrastructure.IImageViewModelFactory;
using ImageViewModel = ImageAnnotationTool.ViewModels.ImageViewModel;

namespace ImageAnnotationTool.Factories;

public class ImageViewModelFactory : IImageViewModelFactory
{
    private readonly IClassListProvider _classListProvider;
    private readonly IDialogWrapper _dialogWrapper;
    private readonly ICommandStack _undoRedoService;
    private readonly IWorkspaceCommandFactory _commandFactory;
    private readonly IAnnotationViewModelFactory _annotationViewModelFactory;
    private readonly IAppModeSettingsProvider _annotationModes;
    private readonly INotificationSettings _notificationSettings;
    private readonly IAppMessenger _appMessenger;
    
    public ImageViewModelFactory(INotificationSettings notificationSettings,
        IClassListProvider classListProvider,
        IDialogWrapper dialogWrapper,
        IAnnotationViewModelFactory annotationViewModelFactory,
        ICommandStack undoRedoService,
        IWorkspaceCommandFactory commandFactory,
        IAppModeSettingsProvider annotationModes,
        IAppMessenger appMessenger)
    {
        _notificationSettings = notificationSettings;
        _classListProvider = classListProvider;
        _dialogWrapper = dialogWrapper;
        _annotationViewModelFactory = annotationViewModelFactory;
        _undoRedoService = undoRedoService;
        _commandFactory = commandFactory;
        _annotationModes = annotationModes;
        _appMessenger = appMessenger;
    }

    public ImageViewModel Create(ImageSpace imageSpace)
    {
        return new ImageViewModel(_classListProvider,
            _notificationSettings,
            _dialogWrapper,
            _annotationViewModelFactory,
            _undoRedoService,
            _commandFactory,
            _annotationModes, 
            _appMessenger,
            imageSpace);
    }
}