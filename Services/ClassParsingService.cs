using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Parsers;

namespace ImageAnnotationTool.Services;

public sealed class ClassParsingService : IClassParser
{
    public async Task<List<string>?> ParseClassYoloAsync(IStorageFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>?> ParseClassCocoAsync(IStorageFile file)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>?> ParseClassVocAsync(IStorageFile file)
    {
        throw new NotImplementedException();
    }
}