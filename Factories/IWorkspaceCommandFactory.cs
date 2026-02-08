using System.Collections.Generic;
using ImageAnnotationTool.Domain.DomainCommands;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Factories;

public interface IWorkspaceCommandFactory
{
    IPersistentCommand AddClass(string className);

    IPersistentCommand AddImage(ImageSource imageSource, ImageMetadata imageMetadata);

    IUndoableCommand ChangeClassAnnotation(Annotation annotation, ClassData? newClass);
    
    IUndoableCommand ModifyNameClass(ClassData classData, string newClassName);

    IUndoableCommand RemoveClass(ClassData classData, ClassData? replacementClassData = null);

    IUndoableCommand RemoveImage(ImageSpace imageSpace);

    IPersistentCommand AddBatchClass(List<string> classes);
    
    IPersistentCommand AddBatchImage(Dictionary<ImageSource, ImageMetadata> images);
    
    IUndoableCommand ChangeClassBatchAnnotation(Dictionary<Annotation, ClassData?> classesAnnotationsDict);
    
    IUndoableCommand ChangeClassBatchAnnotation(List<Annotation> annotations, ClassData? classData);

    IUndoableCommand ChangeClassBatchAnnotation(ClassData oldClass, ClassData? newClass);

    IUndoableCommand RemoveGlobalClassAnnotation(ClassData classData);
    
    IUndoableCommand ModifyNameBatchClass(Dictionary<string, ClassData> namesClassesDict);
    
    IUndoableCommand RemoveBatchClass(Dictionary<ClassData, ClassData?> deleteInsertClassDict);
    
    IUndoableCommand RemoveBatchImage(List<ImageSpace> images);

    IUndoableCommand ChangeGlobalAnnotationVisibility(bool newVisibility);

    IUndoableCommand ChangeGlobalClassVisibility(bool newVisibility);
    
    IUndoableCommand ChangeGlobalAnnotationAnchoring(bool newAnchoring);
}