namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IRenderingInterface
{
    double ZoomBorderScaleX { get; }
    double ZoomBorderScaleY { get; }
    double InverseScaleX { get; }
    double InverseScaleY { get; }
}