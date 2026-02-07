namespace ImageAnnotationTool.Domain.Infrastructure.AnnotationFormats;

public sealed record YoloAnnotation(
    int ClassId,
    double XCenter,
    double YCenter,
    double Width,
    double Height);