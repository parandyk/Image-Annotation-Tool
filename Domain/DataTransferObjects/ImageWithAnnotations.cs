using System.Collections.Generic;

namespace ImageAnnotationTool.Domain.DataTransferObjects;

public sealed record ImageWithAnnotations(
    ImageSnapshot Image,
    IReadOnlyList<AnnotationSnapshot> Annotations);