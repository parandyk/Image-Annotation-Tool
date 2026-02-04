using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ObjectEqualityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is null))
        {
            return BindingOperations.DoNothing;
        }

        if (values.Count != 2 ||
            values[0] is not object a ||
            values[1] is not object b)
        { 
            return BindingOperations.DoNothing;
        }

        return !a.Equals(b);
    }
}