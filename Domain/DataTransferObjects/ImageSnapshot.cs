using System.Collections.Generic;

namespace ImageAnnotationTool.Domain.DataTransferObjects;

public sealed record ImageSnapshot(
    int Id,
    string Name,
    string Path,
    int Width,
    int Height,
    IReadOnlyList<AnnotationSnapshot> Annotations);