using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Factories;

public interface IClassDialogWrapper
{
    DeleteClassDialogViewModel CreateDeleteClassDialog(ClassData classToDelete, ClassData? subsituteClass = null);
    DeleteClassDialogViewModel CreateDeleteClassDialog(List<ClassData> classesToDelete);
    
    RenameClassDialogViewModel CreateRenameClassDialog(ClassData classToRename);
    RenameClassDialogViewModel CreateRenameClassDialog(List<ClassData> classesToRename);
    
    SwapClassInstancesDialogViewModel CreateSwapClassInstancesDialog(ClassData classToSwap, ClassData? substituteClass = null, ImageSpace? image = null);
    
    SwapClassInstancesDialogViewModel CreateSwapClassInstancesDialog(List<ClassData> classesToSwap); //TODO: KEEP?
    
    RemoveClassInstancesDialogViewModel CreateRemoveClassInstancesDialog(ClassData classToRemove, ImageSpace? image = null);
    
    RemoveClassInstancesDialogViewModel CreateRemoveClassInstancesDialog(List<ClassData> classesToRemove); //TODO: KEEP?
    
    UnassignedWarningDialogViewModel CreateUnassignedWarningDialog(ImageSpace? image = null);
}