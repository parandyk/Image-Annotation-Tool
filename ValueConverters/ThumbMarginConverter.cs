using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ThumbMarginConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        
        if (values is null || values.Any(v => v is UnsetValueType))
            return BindingOperations.DoNothing;
        
        if (values.Count < 2 
            || values[0] is not Thickness zoomedThickness 
            || values[1] is not string thumbString)
            return BindingOperations.DoNothing;
        
        
        var marginLeft = 0.0;
        var marginRight = 0.0;
        var marginTop = 0.0;
        var marginBottom = 0.0;
        
        if (thumbString.ToLower().Contains("left"))
        {
            marginLeft = -zoomedThickness.Left / 2;
        }
        else if (thumbString.ToLower().Contains("right"))
        {
            marginRight = -zoomedThickness.Right / 2;
        }
    
        if (thumbString.ToLower().Contains("bottom"))
        {
            marginBottom = -zoomedThickness.Bottom / 2;
        }
        else if (thumbString.ToLower().Contains("top"))
        {
            marginTop = -zoomedThickness.Top / 2;
        }
        
        return new Thickness(marginLeft, marginRight, marginTop, marginBottom);
    }
}