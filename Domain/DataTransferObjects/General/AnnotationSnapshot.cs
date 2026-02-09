namespace ImageAnnotationTool.Domain.DataTransferObjects.General;

public sealed record AnnotationSnapshot(
    string Label,
    double X,
    double Y,
    double Width,
    double Height);