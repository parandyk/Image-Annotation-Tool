using ImageAnnotationTool.Domain.DataTransferObjects;

namespace ImageAnnotationTool.Domain.ExportableObjects;

public sealed class ExportableClass
{
    public string Name { get; }

    public ExportableClass(ClassSnapshot snapshot)
    {
        Name = snapshot.Name;
    }
}