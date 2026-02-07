using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure.AnnotationFormats;

namespace ImageAnnotationTool.Domain.Infrastructure.AnnotationProjections;

public sealed class YoloProjection
{
    public static YoloAnnotation Project(
        Annotation annotation,
        ImageSpace image,
        IReadOnlyDictionary<ClassData, int> classMap)
    {
        var bbox = annotation.Bounds;
        var metadata = image.Metadata;

        return new YoloAnnotation(
            classMap[annotation.ClassInfo],
            (bbox.X1 + bbox.X2) / 2 / metadata.ImagePixelWidth,
            (bbox.Y1 + bbox.Y2) / 2 / metadata.ImagePixelHeight,
            bbox.Width / metadata.ImagePixelWidth,
            bbox.Height / metadata.ImagePixelHeight);
    }
}