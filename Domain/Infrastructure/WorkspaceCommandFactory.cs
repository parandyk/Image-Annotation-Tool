using System.Collections.Generic;
using ImageAnnotationTool.Domain.DomainCommands.Standard;
using ImageAnnotationTool.Domain.DomainCommands.Standard.Batch;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Images.Batch;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace;
using ImageAnnotationTool.Domain.DomainCommands.Undoable.Workspace.Batch;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.Infrastructure;

public class WorkspaceCommandFactory : IWorkspaceCommandFactory
{
    private readonly AnnotationWorkspace _annotationWorkspace;
    private readonly IClassDataPolicy _policy;
    
    public WorkspaceCommandFactory(AnnotationWorkspace annotationWorkspace, IClassDataPolicy policy)
    {
        _annotationWorkspace = annotationWorkspace;
        _policy = policy;
    }

    public IPersistentCommand AddClass(string className)
    {
        return new AddClassCommand(_annotationWorkspace, _policy, className);
    }
    
    public IPersistentCommand AddImage(ImageSource imageSource, ImageMetadata imageMetadata)
    {
        return new AddImageCommand(_annotationWorkspace, imageSource, imageMetadata);
    }

    public IUndoableCommand ChangeClassAnnotation(Annotation annotation, ClassData? newClass)
    {
        return new ChangeClassAnnotationCommand(_annotationWorkspace, annotation, newClass);
    }

    public IUndoableCommand ModifyNameClass(ClassData classData, string newClassName)
    {
        return new ModifyNameClassCommand(_annotationWorkspace, _policy, classData, newClassName);
    }

    public IUndoableCommand RemoveClass(ClassData classForDeletion, ClassData? classForInsertion = null)
    {
        return new RemoveClassCommand(_annotationWorkspace, classForDeletion, classForInsertion);
    }

    public IUndoableCommand RemoveImage(ImageSpace imageSpace)
    {
        return new RemoveImageCommand(_annotationWorkspace, imageSpace);
    }

    public IPersistentCommand AddBatchClass(List<string> classes)
    {
        return new AddBatchClassCommand(_annotationWorkspace, _policy, classes);
    }

    public IPersistentCommand AddBatchImage(Dictionary<ImageSource, ImageMetadata> images)
    {
        return new AddBatchImageCommand(_annotationWorkspace, images);
    }

    public IUndoableCommand ChangeClassBatchAnnotation(Dictionary<Annotation, ClassData?> classesAnnotationsDict)
    {
        return new ChangeClassBatchAnnotationCommand(_annotationWorkspace, classesAnnotationsDict);
    }
    
    public IUndoableCommand ChangeClassBatchAnnotation(List<Annotation> annotations, ClassData? classData)
    {
        return new ChangeClassBatchAnnotationCommand(_annotationWorkspace, annotations, classData);
    }

    public IUndoableCommand ChangeClassBatchAnnotation(ClassData oldClass, ClassData? newClass)
    {
        return new ChangeClassBatchAnnotationCommand(_annotationWorkspace, oldClass, newClass);
    }

    public IUndoableCommand RemoveGlobalClassAnnotation(ClassData classData)
    {
        return new RemoveGlobalBatchAnnotationCommand(_annotationWorkspace, classData);
    }

    public IUndoableCommand ModifyNameBatchClass(Dictionary<string, ClassData> namesClassesDict)
    {
        return new ModifyNameBatchClassCommand(_annotationWorkspace, _policy, namesClassesDict);
    }

    public IUndoableCommand RemoveBatchClass(Dictionary<ClassData, ClassData?> deleteInsertClassDict)
    {
        return new RemoveBatchClassCommand(_annotationWorkspace, deleteInsertClassDict);
    }

    public IUndoableCommand RemoveBatchImage(List<ImageSpace> images)
    {
        return new RemoveBatchImageCommand(_annotationWorkspace, images);
    }
    
    public IUndoableCommand ChangeGlobalAnnotationVisibility(bool newVisibility)
    {
        return new ChangeGlobalVisibilityAnnotationCommand(_annotationWorkspace, newVisibility);
    }
    
    public IUndoableCommand ChangeGlobalClassVisibility(bool newVisibility)
    {
        return new ChangeGlobalVisibilityClassCommand(_annotationWorkspace, newVisibility);
    }

    public IUndoableCommand ChangeGlobalAnnotationAnchoring(bool newAnchoring)
    {
        return new ChangeGlobalAnchoringAnnotationCommand(_annotationWorkspace, newAnchoring);
    }
}