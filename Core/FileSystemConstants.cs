using System.Collections.Generic;

namespace ImageAnnotationTool.Core;

public static class FileSystemConstants
{
    public static IReadOnlyCollection<string> AnnotationFormatList
    {
        get;
    } = ["COCO", "YOLO", "VOC"];
}