using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace ImageAnnotationTool.ValueConverters;

public class BBoxDynamicThicknessConverter : IMultiValueConverter
{
    public object Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is UnsetValueType or null))
            return new Thickness(1.0);
        
        if (values.Count < 6 ||
            !(values[0] is bool borderOn) ||
            !(values[1] is bool dynamicBordersOn) ||
            !(values[2] is double zoomLevel) ||
            !(values[3] is double doubleThickness) ||
            !(values[4] is double bboxWidth) ||
            !(values[5] is double bboxHeight))
            return 0;

        if (!borderOn)
            return new Thickness(0);

        var zoomedThickness = zoomLevel * doubleThickness;
        
        if (!dynamicBordersOn)
            return new Thickness((double)zoomedThickness);;
        
        while (2 * zoomedThickness > bboxWidth || 2 * zoomedThickness > bboxHeight)
        {
            zoomedThickness -= 1;
        }
        
        return new Thickness((double)zoomedThickness!);
    }
}