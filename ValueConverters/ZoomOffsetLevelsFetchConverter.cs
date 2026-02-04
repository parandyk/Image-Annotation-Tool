using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;


namespace ImageAnnotationTool.ValueConverters;

public class ZoomOffsetLevelsFetchConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null 
            || values.Any(v => v is UnsetValueType) 
            || values.Count < 4 
            || values[0] is not double zoomX 
            || values[1] is not double zoomY
            || values[2] is not double offsetX 
            || values[3] is not double offsetY) 
            return BindingOperations.DoNothing;

        return new ValueTuple<double, double, double, double>(zoomX, zoomY,  offsetX, offsetY);
    }
}