using ImageAnnotationTool.Domain.DataTransferObjects;
using ImageAnnotationTool.Domain.DataTransferObjects.General;

namespace ImageAnnotationTool.Domain.ExportableObjects;

public sealed class ExportableClass
{
    public string Name { get; }

    public ExportableClass(ClassSnapshot snapshot)
    {
        Name = snapshot.Name;
    }
}