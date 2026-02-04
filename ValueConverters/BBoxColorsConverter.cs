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
using ImageAnnotationTool.Domain.Entities;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.ValueConverters;

public class BBoxColorsConverter : IMultiValueConverter
{

    public object Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is UnsetValueType))
        {
            return BindingOperations.DoNothing;
        }
        
        if (values.Count < 5 ||
            !(values[0] is ClassData classData) ||
            !(values[1] is bool backgroundsEnabled) ||
            !(values[2] is bool isHighlighted) ||
            !(values[3] is double defaultAlpha) || 
            !(values[4] is double selectedAlpha))
        {
            return new SolidColorBrush(Colors.Transparent);
        }
        
        if (!backgroundsEnabled)
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        var baseColor = classData.HexColor;
        
        double alpha = isHighlighted ? selectedAlpha : defaultAlpha;
        
        return new SolidColorBrush(Color.Parse(baseColor), alpha);
    }
}