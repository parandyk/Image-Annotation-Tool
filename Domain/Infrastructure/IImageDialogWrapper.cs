using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IImageDialogWrapper
{
    DeleteImageDialogViewModel CreateDeleteImageDialog(ImageSpace imageToDelete);
    DeleteImageDialogViewModel CreateDeleteImageDialog(List<ImageSpace> imagesToDelete);
}