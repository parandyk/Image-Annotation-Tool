using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Models;
using ImageAnnotationTool.Parsers;

namespace ImageAnnotationTool.Services;

public sealed class ImageParsingService : IImageParser
{
    public async Task<ValueTuple<ImageSource, ImageMetadata>> ParseImageAsync(IStorageFile file)
    {
        await using var imgStream = await file.OpenReadAsync();
        Bitmap tempImg = new Bitmap(imgStream);
        
        var imageSource = new ImageSource(file.Path.LocalPath);
        var imageMetadata = new ImageMetadata(tempImg.PixelSize.Width, tempImg.PixelSize.Height,
            tempImg.Dpi.X, tempImg.Dpi.Y, tempImg.PixelSize.AspectRatio);
        
        return new ValueTuple<ImageSource, ImageMetadata>(imageSource, imageMetadata);
    }
}