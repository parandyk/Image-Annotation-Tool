using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace ImageAnnotationTool.Parsers;

public interface IClassParser
{
    public Task<List<string>?> ParseClassYoloAsync(IStorageFile file);
    public Task<List<string>?> ParseClassCocoAsync(IStorageFile file);
    public Task<List<string>?> ParseClassVocAsync(IStorageFile file);
}