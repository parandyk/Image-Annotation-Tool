using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ZoomInvarianceConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is UnsetValueType))
        {
            return 1.0;
        }

        if (values.Count < 2)
        {
            return 1.0;
        }
        
        if (values[0] is not double zoomLevel ||
            values[1] is not double baseSize)
            return 1.0;

        return zoomLevel * baseSize;
    }
}