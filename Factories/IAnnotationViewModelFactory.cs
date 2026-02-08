using ImageAnnotationTool.Domain.Entities;
using AnnotationViewModel = ImageAnnotationTool.ViewModels.AnnotationViewModel;

namespace ImageAnnotationTool.Factories;

public interface IAnnotationViewModelFactory
{
    public AnnotationViewModel Create(
        Annotation annotation,
        bool temporary);
}