using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Interfaces;

public interface IFileAccessProvider
{
    
    public Task<List<ValueTuple<ImageSource, ImageMetadata>>?> OpenImageFileAsync();
    public Task<List<string>?> OpenClassFileAsync();
    public Task<List<Annotation>?> OpenAnnotationFileAsync();
    public Task<IStorageFile?> SaveAnnotationFileAsync();
    public Task<string?> OpenOutputFolderAsync();
    public Task<List<string>?> OpenDataFolderAsync();
    public Task<IStorageFile?> SaveClassesFileAsync();
}