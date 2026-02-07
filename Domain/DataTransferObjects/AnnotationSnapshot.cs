using System;
using ImageAnnotationTool.Domain.ValueObjects;

namespace ImageAnnotationTool.Domain.DataTransferObjects;

public sealed record AnnotationSnapshot(
    BoundingBox Box,
    string Label);