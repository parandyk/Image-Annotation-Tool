using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class MathOperationConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string parameterString)
            return BindingOperations.DoNothing;
        
        if (values.Count < 2 ||
            values[0] is not double firstTerm ||
            values[1] is not double secondTerm)
            return BindingOperations.DoNothing;

        return parameterString switch
        {
            "+" => firstTerm + secondTerm,
            "-" => firstTerm - secondTerm,
            "*" => firstTerm * secondTerm,
            "/" => firstTerm / secondTerm,
            _ => BindingOperations.DoNothing
        };
    }
}