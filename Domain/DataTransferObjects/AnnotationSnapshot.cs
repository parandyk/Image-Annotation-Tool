using System;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DataTransferObjects;

public sealed record AnnotationSnapshot(
    string Label,
    double X,
    double Y,
    double Width,
    double Height);