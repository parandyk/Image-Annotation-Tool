using System;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Models;
using ImageAnnotationTool.ViewModels;
using ImageViewModel = ImageAnnotationTool.ViewModels.ImageViewModel;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IImageViewModelFactory
{
    ImageViewModel Create(ImageSpace imageSpace);
}