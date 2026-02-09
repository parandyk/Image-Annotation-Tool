using System;
using System.Collections.Generic;
using System.Linq;
using ImageAnnotationTool.Interfaces;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Services;

public sealed class FileAccessService : IFileAccessProvider
{
    private readonly IStorageProvider _storageProvider;
    private readonly IFileParsingProvider _fileParsingService;
    
    public FileAccessService(IStorageProvider storageProvider,  IFileParsingProvider fileParsingService)
    {
        _storageProvider = storageProvider;
        _fileParsingService = fileParsingService;
    }
    
    public async Task<List<ValueTuple<ImageSource, ImageMetadata>>?> OpenImageFileAsync() //TODO: MULTIPLE FILE HANDLING, MIGRATE TO PARSER METHOD
    {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open image",
            FileTypeFilter = new[] {FileExtensions.ToFilePickerFileType("Image files", FileExtensions.ImageFiles)},
            AllowMultiple = true
        });
     
        var imageList = await GetImagesAsync(files);

        return imageList;
    }
    
    public async Task<List<string>?> OpenClassFileAsync() //TODO: EXTEND TO OTHER FORMATS
    {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open class names file",
            FileTypeFilter = new[] {FileExtensions.ToFilePickerFileType("Class files", FileExtensions.AllClassFiles)},
            AllowMultiple = true,
        });

        var classList = await GetClassesAsync(files);

        return classList;
    }
    
    public async Task<List<Annotation>?> OpenAnnotationFileAsync() //TODO: EXTEND TO OTHER FORMATS
    {
        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open annotations file",
            FileTypeFilter = new[]{FileExtensions.ToFilePickerFileType("Annotation files", FileExtensions.AllAnnotationFiles)},
            AllowMultiple = true
        });

        var annotationList = await GetAnnotationsAsync(files);

        return annotationList;
    }
    
    

    private async Task<List<ValueTuple<ImageSource, ImageMetadata>>?> GetImagesAsync(IReadOnlyList<IStorageFile> files)
    {
        var imageList = new List<ValueTuple<ImageSource, ImageMetadata>>();
        
        foreach (var file in files)
        {
            var fileImage = await _fileParsingService.ParseFileImageAsync(file);
            
            // if (fileImage is null) continue;
            
            imageList.Add(fileImage);
        }
        
        return imageList;
    }
    
    private async Task<List<string>?> GetClassesAsync(IReadOnlyList<IStorageFile> files)
    {
        var classList = new List<string>();
        
        foreach (var file in files)
        {
            var fileClassList = await _fileParsingService.ParseFileClassesAsync(file);
            
            if (fileClassList is null) continue;
            
            classList.AddRange(fileClassList);
        }
        
        return classList;
    }
    
    private async Task<List<Annotation>?> GetAnnotationsAsync(IReadOnlyList<IStorageFile> files)
    {
        var annotationList = new List<Annotation>();
        
        foreach (var file in files)
        {
            var fileAnnotationList = await _fileParsingService.ParseFileAnnotationsAsync(file);
            
            if (fileAnnotationList is null) continue;
            
            annotationList.AddRange(fileAnnotationList);
        }
        
        return annotationList;
    }

    private List<string>? GetFolderPaths(IReadOnlyList<IStorageFolder> folders) //TODO: ASYNC?
    {
        var pathList = new List<string>();

        foreach (var folder in folders)
        {
            var path = _fileParsingService.ParseFolderPath(folder);
            pathList.Add(path);
        }

        return pathList;
    }

    public async Task<IStorageFile?> SaveAnnotationFileAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<string?> OpenOutputFolderAsync()
    {
        var folders = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Pick output folder",
            AllowMultiple = false
        });

        var folder = folders.FirstOrDefault();
        
        if (folder is null) 
            return null;
        
        var path = _fileParsingService.ParseFolderPath(folder);
        
        return path;
    }

    public async Task<List<string>?> OpenDataFolderAsync()
    {
        var folders = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Pick output folder",
            AllowMultiple = false
        });
        
        var paths = GetFolderPaths(folders);
        
        return paths;
    }

    public async Task<IStorageFile?> SaveClassesFileAsync()
    {
        throw new NotImplementedException();
    }
}