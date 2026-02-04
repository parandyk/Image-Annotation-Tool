using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class EnumDisplayNameConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum e)
            return value?.ToString() ?? string.Empty;
        
        var field = e.GetType().GetField(e.ToString());
        var display = field?.GetCustomAttribute<DisplayAttribute>();

        return display?.Name ?? e.ToString();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}