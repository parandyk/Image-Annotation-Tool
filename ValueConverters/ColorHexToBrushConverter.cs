using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ImageAnnotationTool.ValueConverters;

public class ColorHexToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string colorHex)
        {
            return BindingOperations.DoNothing;
        }
        
        return new SolidColorBrush(Color.Parse(colorHex));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush colorBrush)
        {
            return BindingOperations.DoNothing;
        }
        
        return colorBrush.Color.ToString();
    }
}