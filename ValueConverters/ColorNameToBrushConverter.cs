using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ImageAnnotationTool.ValueConverters;

public class ColorNameToBrushConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is UnsetValueType))
        {
            return BindingOperations.DoNothing;
        }

        string colorFromString;
        
        if (parameter is AvaloniaDictionary<string, string> parameterColorMap)
        {
            if ((values.Count < 1) || !(values[0] is string className))
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            
            colorFromString = parameterColorMap.FirstOrDefault(c => c.Key == className).Value;
        }
        else
        {
            if (values.Count < 2 ||
                !(values[0] is string className) ||
                !(values[1] is AvaloniaDictionary<string, string> colorMap))
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            colorFromString = colorMap.FirstOrDefault(c => c.Key == className).Value;
        }

        return new SolidColorBrush(Color.Parse(colorFromString));
    }
}