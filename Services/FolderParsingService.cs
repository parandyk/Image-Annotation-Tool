using Avalonia.Platform.Storage;
using ImageAnnotationTool.Parsers;

namespace ImageAnnotationTool.Services;

public class FolderParsingService : IFolderParser
{
    public string ParseFolderPath(IStorageFolder folder)
    {
        return folder.Path.LocalPath;
    }
}