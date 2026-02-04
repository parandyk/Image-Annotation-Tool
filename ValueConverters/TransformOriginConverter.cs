using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace ImageAnnotationTool.ValueConverters;

public class TransformOriginConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Any(v => v is UnsetValueType))
            return BindingOperations.DoNothing;

        if (values.Count < 2
            || values[0] is not HorizontalAlignment horizontalAlignment
            || values[1] is not VerticalAlignment verticalAlignment)
            return BindingOperations.DoNothing;

        double px = 0.0;
        double py = 0.0;

        if (horizontalAlignment == HorizontalAlignment.Left)
        {
            px = 0;
        }
        else if (horizontalAlignment == HorizontalAlignment.Center)
        {
            px = 0.5;
        }
        else if (horizontalAlignment == HorizontalAlignment.Right)
        {
            px = 1;
        }

        if (verticalAlignment == VerticalAlignment.Top)
        {
            py = 0;
        }
        else if (verticalAlignment == VerticalAlignment.Center)
        {
            py = 0.5;
        }
        else if (verticalAlignment == VerticalAlignment.Bottom)
        {
            py = 1;
        }
        
        return new RelativePoint(new Avalonia.Point(px, py), RelativeUnit.Relative);
    }
}