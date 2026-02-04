using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ControlBoundsAccessConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Rect controlBounds)
            return null;
        
        var controlBoundsWidth = controlBounds.Width;
        var controlBoundsHeight = controlBounds.Height;
        var controlBoundsY = controlBounds.Y; 
        var controlBoundsX = controlBounds.X;

        return new ValueTuple<double, double, double, double>(controlBoundsWidth, controlBoundsHeight, controlBoundsX, controlBoundsY);
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}