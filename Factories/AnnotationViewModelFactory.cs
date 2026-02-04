using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure;
using ImageAnnotationTool.Interfaces;
using AnnotationViewModel = ImageAnnotationTool.ViewModels.AnnotationViewModel;
using IAnnotationViewModelFactory = ImageAnnotationTool.Domain.Infrastructure.IAnnotationViewModelFactory;
using IClassListProvider = ImageAnnotationTool.Domain.Infrastructure.IClassListProvider;

namespace ImageAnnotationTool.Factories;

public class AnnotationViewModelFactory : IAnnotationViewModelFactory
{
    private readonly ITransientAppModeSettings _modeSettings;
    private readonly IAppMessenger _messenger;
    
    public AnnotationViewModelFactory(
        ITransientAppModeSettings modeSettings,
        IAppMessenger messenger)
    {
        _modeSettings = modeSettings;
        _messenger = messenger;
    }

    public AnnotationViewModel Create(
        Annotation annotation,
        bool temporary)
    {
        return new AnnotationViewModel(
            _modeSettings,
            _messenger,
            annotation,
            temporary);
    }
}