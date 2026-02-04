using System;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Models;
using ImageAnnotationTool.ViewModels;
using AnnotationViewModel = ImageAnnotationTool.ViewModels.AnnotationViewModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IAnnotationViewModelFactory
{
    public AnnotationViewModel Create(
        Annotation annotation,
        bool temporary);
}