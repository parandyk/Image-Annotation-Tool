using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ControlBoundsSingleValueAccessConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Rect controlBounds)
            return null;

        if (parameter is not string)
            return null;

        return parameter.ToString()!.ToLower() switch
        {
            "0" or "x" => controlBounds.X,
            "1" or "y" => controlBounds.Y,
            "2" or "w" or "width" => controlBounds.Width,
            "3" or "h" or "height" => controlBounds.Height,
            _ => null
        };
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}