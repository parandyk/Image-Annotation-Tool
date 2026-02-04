using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ControlDimensionsAccessConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Rect && value is not Size)
            return null;

        double width = 0;
        double height = 0;
        
        if (value is Size size)
        {
            width = size.Width;
            height = size.Height;
        }
        else if (value is Rect rect)
        {
            width = rect.Width;
            height = rect.Height;
        }

        return new ValueTuple<double, double>(width, height);
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}