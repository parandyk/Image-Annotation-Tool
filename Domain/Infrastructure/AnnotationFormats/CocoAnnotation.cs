namespace ImageAnnotationTool.Domain.Infrastructure.AnnotationFormats;

public record CocoAnnotation(
    int CategoryId,
    int ImageId,
    double[] Bbox,
    double Area);