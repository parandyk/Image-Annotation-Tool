using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class ThumbVisibilityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Any(v => v is UnsetValueType))
            return BindingOperations.DoNothing;
                    
        if (values.Count < 5 
            || values[0] is not double width 
            || values[1] is not double height
            || values[2] is not double thumbWidth 
            || values[3] is not double thumbHeight
            || values[4] is not string thumbString)
            return BindingOperations.DoNothing;

        if (thumbString.ToLower().Contains("left") || thumbString.ToLower().Contains("right"))
        {
            if (thumbString.ToLower().Contains("center"))
            {
                if (height <= thumbHeight * 3.0)
                {
                    return false;
                }
            }
            
            if (width <= thumbWidth)
                return false;
        }
        
        if (thumbString.ToLower().Contains("top") || thumbString.ToLower().Contains("bottom"))
        {
            if (thumbString.ToLower().Contains("center"))
            {
                if (width <= thumbWidth * 3.0)
                {
                    return false;
                }
            }
            
            if (height <= thumbHeight)
                return false;
        }
        
        return true;
    }
}