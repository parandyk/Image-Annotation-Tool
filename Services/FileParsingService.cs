using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Models;
using ImageAnnotationTool.Parsers;

namespace ImageAnnotationTool.Services;

public sealed class FileParsingService : IFileParsingProvider
{
    private readonly IImageParser _imageParsingService;
    private readonly IClassParser _classParsingService;
    private readonly IAnnotationParser _annotationParsingService;

    public FileParsingService(IImageParser imageParsingService,  IClassParser classParsingService, IAnnotationParser annotationParsingService)
    {
        _imageParsingService = imageParsingService;
        _classParsingService = classParsingService;
        _annotationParsingService = annotationParsingService;
    }
    
    public async Task<List<string>?> ParseFileClassesAsync(IStorageFile file)
    {
        var ext = Path.GetExtension(file.Path.ToString());

        return ext switch
        {
            _ when FileExtensions.YoloClass.Contains(ext) => await _classParsingService.ParseClassYoloAsync(file),
            _ when FileExtensions.Coco.Contains(ext) => await _classParsingService.ParseClassCocoAsync(file),
            _ when FileExtensions.PascalVoc.Contains(ext) => await _classParsingService.ParseClassVocAsync(file),
            _ => throw new NotImplementedException()
        };
    }
    
    public async Task<List<Annotation>?> ParseFileAnnotationsAsync(IStorageFile file)
    {
        var ext = Path.GetExtension(file.Path.ToString());

        return ext switch
        {
            _ when FileExtensions.YoloAnnotation.Contains(ext) => await _annotationParsingService.ParseAnnotationYoloAsync(file),
            _ when FileExtensions.Coco.Contains(ext) => await _annotationParsingService.ParseAnnotationCocoAsync(file),
            _ when FileExtensions.PascalVoc.Contains(ext) => await _annotationParsingService.ParseAnnotationVocAsync(file),
            _ => throw new NotImplementedException()
        };
    }

    public async Task<ValueTuple<ImageSource, ImageMetadata>> ParseFileImageAsync(IStorageFile file)
    {
        return await _imageParsingService.ParseImageAsync(file);
    }
}