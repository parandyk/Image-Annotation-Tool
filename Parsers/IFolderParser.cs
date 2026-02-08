using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace ImageAnnotationTool.Parsers;

public interface IFolderParser
{
    string ParseFolderPath(IStorageFolder folder);
}