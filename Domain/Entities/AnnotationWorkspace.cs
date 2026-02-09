using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.DataTransferObjects.General;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Domain.Entities;

public sealed class AnnotationWorkspace : ObservableObject, IWorkspaceDomainInterface
{
    public AnnotationWorkspace()
    {
        AddClass(new ClassData("unassigned", "#FF000000"));
        _unassignedId = GetClass("unassigned").Id;
        ClassRunningCount = _classes.Count;
    }
    
    public event Action? ImageChanged;
    public event Action? ClassChanged;
    public event Action? AnnotationChanged;
    
    private readonly Guid _unassignedId;
    
    private readonly ObservableCollection<ImageSpace> _images = new();
    public ReadOnlyObservableCollection<ImageSpace> Images => 
        new ReadOnlyObservableCollection<ImageSpace>(_images);
    
    private readonly ObservableCollection<ClassData> _classes = new();
    
    public ReadOnlyObservableCollection<ClassData> Classes => 
        new ReadOnlyObservableCollection<ClassData>(_classes);
    
    public ReadOnlyObservableCollection<Annotation> AllAnnotations => 
        new ReadOnlyObservableCollection<Annotation>(
            new ObservableCollection<Annotation>(_images.SelectMany(img => 
                img.Annotations).ToList()));
    
    public int ClassRunningCount { get; private set; } = 0;
    
    public int ImageRunningCount { get; private set; } = 0;

    public ImageSnapshot ImageToSnapshot(ImageSpace image, bool includeFallback)
    {
        List<AnnotationSnapshot> annotations;

        if (includeFallback)
        {
            annotations = image.Annotations
                .Select(a => AnnotationToSnapshot(a)).ToList();
        }
        else
        {
            annotations = image.Annotations
                .Where(a => !IsClassDefault(a.ClassInfo))
                .Select(a => AnnotationToSnapshot(a)).ToList();
        }

        return new ImageSnapshot(
            image.ExportId,
            image.Source.ImageName,
            image.Source.ImagePath,
            image.Metadata.ImagePixelWidth,
            image.Metadata.ImagePixelHeight,
            annotations);
    }
    
    public ClassSnapshot ClassToSnapshot(ClassData classData)
    {
        return new ClassSnapshot(classData.Name);
    }
    
    public AnnotationSnapshot AnnotationToSnapshot(Annotation annotation)
    {
        var bounds = annotation.Bounds;
        return new AnnotationSnapshot(
            annotation.ClassInfo.Name,
            bounds.X1,
            bounds.Y1,
            bounds.Width,
            bounds.Height);
    }
    
    internal void IncrementClassRunningCount()
    {
        ClassRunningCount++;
    }
    
    internal void IncrementImageRunningCount()
    {
        ImageRunningCount++;
    }
    
    internal ClassData GetClass(string className)
    {
        if (!ClassExists(className))
        {
            throw new ConstraintException("Class not found.", new ArgumentException(null, nameof(className)));
        }
        
        var cls = _classes.First(cls => cls.Name == className);
        return cls;
    }
    
    internal ClassData GetClass(Guid classId)
    {
        if (!ClassExists(classId))
        {
            throw new ConstraintException("Class not found.", new ArgumentException(null, nameof(classId)));
        }
        
        var cls = _classes.First(cls => cls.Id == classId);
        return cls;
    }

    public int GetClassCount()
    {
        return _classes.Count;
    }

    public bool TryGetClassById(Guid classId, out ClassData classData)
    {
        classData = _classes.FirstOrDefault(classData => classData.Id == classId);
        return classData != null;
    }

    public bool TryGetClassByName(string className, out ClassData classData)
    {
        classData = _classes.FirstOrDefault(classData => Equals(classData.Name, className));
        return classData != null;
    }

    public ClassData GetDefaultClass()
    {
        return GetClass(_unassignedId);
    }

    internal ImageSpace GetImage(string imagePath)
    {
        if (!ImageExists(imagePath))
        {
            throw new ConstraintException("Image not found.", new ArgumentException(null, nameof(imagePath)));
        }
        
        var img = _images.First(img => Equals(img.Source.ImagePath, imagePath));
        return img;
    }
    
    internal ImageSpace GetImage(Guid imageId)
    {
        if (!ImageExists(imageId))
        {
            throw new ConstraintException("Image not found.", new ArgumentException(null, nameof(imageId)));
        }
        
        var img = _images.First(img => img.Id == imageId);
        return img;
    }

    public int GetImageCount()
    {
        return _images.Count;
    }

    public bool TryGetImageById(Guid imageId, out ImageSpace imageSpace)
    {
        imageSpace = _images.FirstOrDefault(imageSpace => imageSpace.Id == imageId);
        return imageSpace != null;
    }

    public bool TryGetImageByPath(string imagePath, out ImageSpace imageSpace)
    {
        imageSpace = _images.FirstOrDefault(imageSpace => Equals(imageSpace.Source.ImagePath, imagePath));
        return imageSpace != null;
    }
    
    internal void AddClass(ClassData classData)
    {
        if (ClassExists(classData))
        {
            throw new ConstraintException("Class already exists.", new ArgumentException(null, nameof(classData)));
        }
        
        if (ClassExists(classData.Name))
        {
            throw new ConstraintException("Class with specified name already exists.", new ArgumentException(null, nameof(classData)));
        }

        classData.ClassChanged += OnClassChanged;
        _classes.Add(classData);
        ClassChanged?.Invoke();
    }

    internal void RemoveClass(ClassData classData)
    {
        if (!ClassExists(classData))
        {
            throw new ConstraintException("Class not found.", new ArgumentException(null, nameof(classData)));
        }
        
        if (IsClassDefault(classData))
        {
            throw new ConstraintException("Deleting default class \"unassigned\" is forbidden.", new ArgumentException(null, nameof(classData)));
        }
        
        classData.ClassChanged -= OnClassChanged;
        _classes.Remove(classData);
        ClassChanged?.Invoke();
    }
    
    private void OnClassChanged()
    {
        ClassChanged?.Invoke();
    }

    internal void AddImage(ImageSpace imageSpace)
    {
        if (ImageExists(imageSpace))
        {
            throw new ConstraintException("Image already exists.", new ArgumentException(null, nameof(imageSpace)));
        }

        if (ImageExists(imageSpace.Source.ImagePath))
        {
            throw new ConstraintException("Image with the same path already exists.", new ArgumentException(null, nameof(imageSpace.Source.ImagePath)));
        }
        
        if (ImageWithNameExists(imageSpace.Source.ImageName))
        {
            throw new ConstraintException("Image with the same name already exists.", new ArgumentException(null, nameof(imageSpace.Source.ImageName)));
        }

        imageSpace.AnnotationChanged += OnAnnotationChanged;
        _images.Add(imageSpace);
        ImageChanged?.Invoke();
    }
    
    internal void RemoveImage(ImageSpace imageSpace)
    {
        if (!ImageExists(imageSpace))
        {
            throw new ConstraintException("Image not found.", new ArgumentException(null, nameof(imageSpace)));
        }
        
        imageSpace.AnnotationChanged -= OnAnnotationChanged;
        _images.Remove(imageSpace);
        ImageChanged?.Invoke();
    }

    private void OnAnnotationChanged()
    {
        AnnotationChanged?.Invoke();
    }
    
    internal void InsertImage(int index, ImageSpace imageSpace)
    {
        if (ImageExists(imageSpace))
        {
            throw new ConstraintException("Image already exists.", new ArgumentException(null, nameof(imageSpace)));
        }

        if (ImageExists(imageSpace.Source.ImagePath))
        {
            throw new ConstraintException("Image with the same path already exists.", new ArgumentException(null, nameof(imageSpace.Source.ImagePath)));
        }
        
        if (ImageWithNameExists(imageSpace.Source.ImageName))
        {
            throw new ConstraintException("Image with the same name already exists.", new ArgumentException(null, nameof(imageSpace.Source.ImageName)));
        }
        
        _images.Insert(index, imageSpace);
        ImageChanged?.Invoke();
    }
    
    internal void InsertClass(int index, ClassData classData)
    {
        if (ClassExists(classData))
        {
            throw new ConstraintException("Class already exists.", new ArgumentException(null, nameof(classData)));
        }
        
        if (ClassExists(classData.Name))
        {
            throw new ConstraintException("Class with specified name already exists.", new ArgumentException(null, nameof(classData.Name)));
        }
        
        _classes.Insert(index, classData);
        ClassChanged?.Invoke();
    }
    
    public bool ClassExists(ClassData classData)
    {
        return _classes.Contains(classData);
    }

    public bool ClassExists(string className)
    {
        return _classes.Any(cls => Equals(cls.Name, className));
    }
    
    public bool ClassExists(Guid classId)
    {
        return _classes.Any(cls => cls.Id == classId);
    }

    internal bool IsClassDefault(ClassData classData)
    {
        return classData.Id == _unassignedId;
    }

    public bool ImageExists(ImageSpace imageSpace)
    {
        return _images.Contains(imageSpace);
    }
    
    public bool ImageExists(Guid imageId)
    {
        return _images.Any(img => img.Id == imageId);
    }
    
    public bool ImageWithNameExists(string imageName)
    {
        return _images.Any(img => Equals(img.Source.ImageName, imageName));
    }

    public bool ImageExists(string imagePath)
    {
        return _images.Any(img => Equals(img.Source.ImagePath, imagePath));
    }
    
    public bool AnnotationExists(Guid annotationId)
    {
        return AllAnnotations.Any(annotation => annotation.Id == annotationId);
    }

    public int GetAnnotationCount()
    {
        return AllAnnotations.Count();
    }

    public bool AnnotationExists(Annotation annotation)
    {
        return AllAnnotations.Contains(annotation);
    }

    internal Annotation GetAnnotation(Guid annotationId)
    {
        if (!AnnotationExists(annotationId))
        {
            throw new ConstraintException("Annotation not found.", new ArgumentException(null, nameof(annotationId)));
        }
        
        return AllAnnotations.First(annotation => annotation.Id == annotationId); //?
    }
    
    public bool TryGetAnnotationById(Guid annotationId, out Annotation annotation)
    {
        annotation = AllAnnotations.FirstOrDefault(annotation => annotation.Id == annotationId);
        return annotation != null;
    }
}