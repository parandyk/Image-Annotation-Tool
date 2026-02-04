using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class BBoxVisibilityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Any(v => v is UnsetValueType))
            return BindingOperations.DoNothing;
        
        if (values.Count < 2
            || values[0] is not bool classVisibility
            || values[1] is not bool individualVisibility)
            return BindingOperations.DoNothing;
        
        return classVisibility && individualVisibility;
    }
}