using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Parsers;

public interface IImageParser
{
    public Task<ValueTuple<ImageSource, ImageMetadata>> ParseImageAsync(IStorageFile file);
}
