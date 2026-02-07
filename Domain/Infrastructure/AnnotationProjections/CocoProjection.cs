using System.Collections.Generic;
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Domain.Infrastructure.AnnotationFormats;

namespace ImageAnnotationTool.Domain.Infrastructure.AnnotationProjections;

public sealed class CocoProjection
{
    public static CocoAnnotation Project(
        Annotation annotation,
        ImageSpace image,
        IReadOnlyDictionary<ClassData, int> classMap,
        IReadOnlyDictionary<ImageSpace, int> imageMap)
    {
        var bbox = annotation.Bounds;
        var metadata = image.Metadata;

        return new CocoAnnotation(
            classMap[annotation.ClassInfo],
            imageMap[image],
            [bbox.X1, bbox.Y1, bbox.Width, bbox.Height],
            bbox.Width * bbox.Height);
    }
}