using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class StringEqualityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is null))
        {
            return BindingOperations.DoNothing;
        }

        if (values.Count != 2 ||
            values[0] is not string str1 ||
            values[1] is not string str2)
        { 
                return BindingOperations.DoNothing;
        }

        if (parameter is false)
        {
            return !Equals(str1, str2);
        }

        return Equals(str1, str2);
    }
}