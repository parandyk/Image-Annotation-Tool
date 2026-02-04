using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ImageAnnotationTool.Enums;

namespace ImageAnnotationTool.ValueConverters;

public class AnnotationAddingModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AnnotationAddingMode mode)
            return false;

        switch (mode)
        {
            default:
            case AnnotationAddingMode.ClickDraw:
                return false;
            case AnnotationAddingMode.DragDraw:
                return true;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool booleanValue)
            return AnnotationAddingMode.ClickDraw;

        switch (booleanValue)
        {
            case true:
                return AnnotationAddingMode.DragDraw;
            case false:
                return AnnotationAddingMode.ClickDraw;
        }
    }
}