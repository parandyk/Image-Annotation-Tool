using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using AtomUI.Desktop.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public sealed class StatisticsAggregator : ObservableObject, IStatisticsAggregator
{
    private readonly IWorkspaceDomainInterface _workspace;
    
    public StatisticsAggregator(IWorkspaceDomainInterface workspaceDomainInterface)
    {
        _workspace = workspaceDomainInterface;
        _workspace.AnnotationChanged += OnAnnotationsChanged;
        _workspace.ClassChanged += OnClassesChanged;
        _workspace.ImageChanged += OnImagesChanged;
    }

    private void OnImagesChanged()
    {
        // OnPropertyChanged(nameof(ImageTotalCount));
        OnPropertyChanged(nameof(ClassesPerImageCounts));
        OnPropertyChanged(nameof(SpecificClassPerImageCounts));
        OnPropertyChanged(nameof(AnnotationsPerImageCounts));
    }

    private void OnClassesChanged()
    {
        OnPropertyChanged(nameof(GlobalClassVisibility));
        OnPropertyChanged(nameof(AnnotationsPerClassCounts));
        OnPropertyChanged(nameof(SpecificClassPerImageCounts));
    }

    private void OnAnnotationsChanged()
    {
        OnPropertyChanged(nameof(GlobalAnnotationVisibility));
        OnPropertyChanged(nameof(GlobalAnnotationAnchoring));
        OnPropertyChanged(nameof(ClassesPerImageCounts));
        OnPropertyChanged(nameof(AnnotationsPerImageCounts));
        OnPropertyChanged(nameof(AnnotationsPerClassCounts));
        OnPropertyChanged(nameof(SpecificClassPerImageCounts));
    }

    public bool? GlobalClassVisibility
    {
        get
        {
            if (_workspace.Classes.Count == 0)
                return true;
            
            if (_workspace.Classes.All(c => c.IsVisible) || 
                _workspace.Classes.All(c => !c.IsVisible))
            {
                return _workspace.Classes.All(c => c.IsVisible);
            }
    
            return null;
        }
    }
    
    public bool? GlobalAnnotationVisibility
    {
        get
        {
            if (_workspace.AllAnnotations.Count == 0)
                return true;
            
            if (_workspace.AllAnnotations.All(annotation => annotation.IsVisible) || 
                _workspace.AllAnnotations.All(annotation => !annotation.IsVisible))
            {
                return _workspace.AllAnnotations.All(annotation => annotation.IsVisible);
            }
    
            return null;
        }
    }

    public bool? GlobalAnnotationAnchoring
    {
        get
        {
            if (_workspace.AllAnnotations.Count == 0)
                return false;
            
            if (_workspace.AllAnnotations.All(annotation => annotation.IsAnchored) ||
                _workspace.AllAnnotations.All(annotation => !annotation.IsAnchored))
            {
                return _workspace.AllAnnotations.All(annotation => annotation.IsAnchored);
            }

            return null;
        }
    }
    
    public IReadOnlyDictionary<Guid, int> AnnotationsPerImageCounts 
    {
        get
        {
            return _workspace.Images.Select(img => 
                    new KeyValuePair<Guid, int>(img.Id, img.Annotations.Count)).OrderBy(kvp => kvp.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
}

    public IReadOnlyDictionary<Guid, int> ClassesPerImageCounts
    {
        get
        {
            return _workspace.Images.Select(img => 
                    new KeyValuePair<Guid, int>(img.Id, img.Annotations.Select(ann => ann.ClassInfo)
                        .ToList().Distinct().Count())).OrderBy(kvp => kvp.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }


    public IReadOnlyDictionary<Guid, int> AnnotationsPerClassCounts
    {
        get
        {
            return _workspace.Classes.Select(c =>
                        new KeyValuePair<Guid, int>(c.Id, _workspace.AllAnnotations.Where(ann => ann.ClassInfo.Id == c.Id)
                            .ToList()
                            // .Distinct()
                            .Count())).OrderBy(kvp => kvp.Value)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    
    //TODO REMOVE?
    public IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, int>> SpecificClassPerImageCounts
    {
        get
        {
            return _workspace.Images.Select(img =>
                    new KeyValuePair<Guid, IReadOnlyDictionary<Guid, int>>(img.Id,
                        _workspace.Classes.Select(c =>
                                new KeyValuePair<Guid, int>(c.Id, img.Annotations.Select(ann =>
                                    ann.ClassInfo.Id == c.Id).ToList().Count())).OrderBy(kvp => kvp.Value)
                            .ToDictionary(pair => pair.Key, pair => pair.Value)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
    
    // public int AnnotationTotalCount => _workspace.AllAnnotations.Count;
    // public int ClassTotalCount => _workspace.Classes.Count;
    // public int ImageTotalCount => _workspace.Images.Count;
}