namespace ImageAnnotationTool.Domain.Infrastructure.AnnotationFormats;

public sealed record VocObject(
    string Name,
    double XMin,
    double YMin,
    double XMax,
    double YMax);