using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Interfaces;

public interface IFileParsingProvider
{
    public string ParseFolderPath(IStorageFolder folder);
    public Task<List<string>?> ParseFileClassesAsync(IStorageFile file);
    public Task<List<Annotation>?> ParseFileAnnotationsAsync(IStorageFile file);
    public Task<ValueTuple<ImageSource, ImageMetadata>> ParseFileImageAsync(IStorageFile file);
}