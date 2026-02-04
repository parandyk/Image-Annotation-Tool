using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Models;
using ImageAnnotationTool.Parsers;

namespace ImageAnnotationTool.Services;

public sealed class AnnotationParsingService : IAnnotationParser
{
    public async Task<List<Annotation>?> ParseAnnotationYoloAsync(IStorageFile file)
    {
        throw new NotImplementedException();
    }
    public async Task<List<Annotation>?> ParseAnnotationCocoAsync(IStorageFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Annotation>?> ParseAnnotationVocAsync(IStorageFile file)
    {
        throw new NotImplementedException();
    }
}