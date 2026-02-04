using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ThicknessConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;
        
        if (value is double) 
            return new Thickness((double)value);
        if (value is Thickness)
            return (Thickness)value;
        if (value is int) 
            return new Thickness((int)value);
        
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}