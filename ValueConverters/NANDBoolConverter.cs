using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.ValueConverters;

public class NANDBoolConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Count < 2 || values.Any(v => v is null))
            return null;
        
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] is false)
                return true;
        }

        return false;
    }
}