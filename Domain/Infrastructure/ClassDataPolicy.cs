using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImageAnnotationTool.Domain.Entities;

namespace ImageAnnotationTool.Domain.Infrastructure;

public sealed partial class ClassDataPolicy : IClassDataPolicy
{
    public string AssignClassColorHex(int classCount)
    {
        var idx = classCount > 0 ? (classCount - 1) % ColorDictionary.Count : 0;
        var newClassColorHex = ColorDictionary.ElementAtOrDefault(idx).Value;
        return newClassColorHex;
    }
    
    public IReadOnlyDictionary<string, string> ColorDictionary => new Dictionary<string, string>
    {
        { "Red", "#FFE6194B" },
        { "Green", "#FF3CB44B" },
        { "Yellow", "#FFFFE119" },
        { "Blue", "#FF0082C8" },
        { "Orange", "#FFF58230" },
        { "Purple", "#FF911EB4" },
        { "Cyan", "#FF46F0F0" },
        { "Magenta", "#FFF032E6" },
        { "Lime", "#FFD2F53C" },
        { "Pink", "#FFFABED4" },
        { "Teal", "#FF008080" },
        { "Lavender", "#FFDCC0FF" },
        { "Brown", "#FFAA6E28" },
        { "Beige", "#FFFFFAC8" },
        { "Maroon", "#FF800000" },
        { "Mint", "#FFAFFFFF" },
        { "Olive", "#FF808000" },
        { "Navy", "#FF000080" },
        { "Grey", "#FF808080" }
    };
    
    public bool IsValid(ClassData classData)
    {
        if (IsColorValid(classData.HexColor) && IsNameValid(classData.Name))
        {
            return true;
        }
        
        return false;
    }

    public bool IsColorValid(ClassData classData)
    {
        if (string.IsNullOrWhiteSpace(classData.HexColor) || 
            string.IsNullOrEmpty(classData.HexColor) || 
            !IsColorValidHex(classData.HexColor))
        {
            return false;
        }

        return ColorDictionary.Values.Contains(classData.HexColor);
    }
    
    public bool IsColorValid(string? classColorHex)
    {
        if (string.IsNullOrWhiteSpace(classColorHex) 
            || string.IsNullOrEmpty(classColorHex) 
            || !IsColorValidHex(classColorHex))
        {
            return false;
        }
        
        return ColorDictionary.Values.Contains(classColorHex);
    }

    public bool IsColorValidHex(string classColorHex)
    {
        return HexValidationRegex().IsMatch(classColorHex);
    }

    public bool IsNameValid(string? className)
    {
        if (string.IsNullOrWhiteSpace(className) || string.IsNullOrEmpty(className))
        {
            return false;
        }
        
        return true;
    }
    
    public bool IsNameValid(ClassData classData)
    {
        if (string.IsNullOrWhiteSpace(classData.Name) || string.IsNullOrEmpty(classData.Name))
        {
            return false;
        }
        
        return true;
    }
    
    private string SanitizeClassName(string className)
    {
        if (!className.Any(char.IsAsciiLetter) && className.Any(char.IsDigit))
        {
            className = "class_" + className;
        }
        
        var cleanedName = SanitizationRegex().Replace(className, "_");
        
        return cleanedName;
    }
    
    public bool TrySanitizeClassName(string className, out string sanitizedName)
    {
        if (!IsNameValid(className))
        {
            sanitizedName = className;
            return false;
        }
        
        
        sanitizedName = SanitizationRegex().Replace(className, "_");
        sanitizedName = Regex.Replace(sanitizedName, @"_+", "_");
        sanitizedName = sanitizedName.Trim('_');
        
        if (!sanitizedName.Any(char.IsAsciiLetter) || sanitizedName.All(char.IsDigit))
        {
            sanitizedName = "class_" + sanitizedName;
        }
        return true;
    }
    
    [GeneratedRegex(@"[^A-Za-z0-9_\-]")]
    private static partial Regex SanitizationRegex();

    [GeneratedRegex(@"^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$")]
    private static partial Regex HexValidationRegex();
}