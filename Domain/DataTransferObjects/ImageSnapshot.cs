namespace ImageAnnotationTool.Domain.DataTransferObjects;

public sealed record ImageSnapshot(
    string Id,
    string Name,
    int Width,
    int Height);