using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.DataTransferObjects.General;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IWorkspaceDomainImageInterface : INotifyPropertyChanged
{
    ReadOnlyObservableCollection<ImageSpace> Images { get; }

    event Action? ImageChanged;
    
    ImageSnapshot ImageToSnapshot(ImageSpace image, bool includeFallback);
    
    bool ImageExists(string imagePath);
    bool ImageExists(Guid imageId);
    bool ImageExists(ImageSpace imageSpace);
    bool ImageWithNameExists(string imageName);
    
    int GetImageCount();
    
    bool TryGetImageById(Guid imageId, out ImageSpace imageSpace);
    bool TryGetImageByPath(string imagePath, out ImageSpace imageSpace);
}