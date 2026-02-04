using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public interface IClassDataPolicy : IColorAssigner
{
    bool IsValid(ClassData classData);
    
    bool IsColorValid(ClassData classData);
    bool IsColorValid(string classColorHex);
    
    bool IsNameValid(ClassData classData);
    bool IsNameValid(string classColorHex);
    
    bool TrySanitizeClassName(string className, out string sanitizedName);
}