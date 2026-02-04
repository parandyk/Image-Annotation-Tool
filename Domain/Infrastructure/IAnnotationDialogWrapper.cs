using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IAnnotationDialogWrapper
{
    ChangeClassDialogViewModel CreateChangeAnnotationClassDialog(Annotation annotationToModify);
    ChangeClassDialogViewModel CreateChangeAnnotationClassDialog(List<Annotation> annotationsToModify);
    
    DeleteAnnotationDialogViewModel CreateDeleteAnnotationDialog(Annotation annotationToDelete);
    
    DeleteAnnotationDialogViewModel CreateDeleteAnnotationDialog(List<Annotation> annotationsToDelete);
}