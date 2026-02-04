using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class SnappingConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Any(v => v is null))
            return null;
    
        if (values.Count != 2 ||
            values[0] is not double rawCoordinate ||
            values[1] is not Visual host)
            return null;
        
        var scale = TopLevel.GetTopLevel(host)?.RenderScaling ?? 1.0;
        var scaledCoordinate = Math.Round(rawCoordinate * scale) / scale;
        
        return scaledCoordinate;
    }
}