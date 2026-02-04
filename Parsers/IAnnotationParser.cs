using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Parsers;

public interface IAnnotationParser
{
    public Task<List<Annotation>?> ParseAnnotationYoloAsync(IStorageFile file);
    public Task<List<Annotation>?> ParseAnnotationCocoAsync(IStorageFile file);
    public Task<List<Annotation>?> ParseAnnotationVocAsync(IStorageFile file);
}