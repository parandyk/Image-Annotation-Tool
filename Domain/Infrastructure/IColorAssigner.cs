using System.Collections.Generic;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IColorAssigner
{
    string AssignClassColorHex(int classCount);
    IReadOnlyDictionary<string, string> ColorDictionary { get; }
}