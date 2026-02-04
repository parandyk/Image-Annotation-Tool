using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Threading;

namespace ImageAnnotationTool.ValueConverters;

public class PointerPositionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case PointerPressedEventArgs e:
            {
                if (e.Source is not Visual || parameter is not Visual visual)
                {
                    return AvaloniaProperty.UnsetValue;
                }

                if (!e.Properties.IsLeftButtonPressed)
                    return null;
                
                var (x, y) = e.GetPosition(Dispatcher.UIThread.Invoke(() => visual));

                return (x, y);
            }
            case PointerReleasedEventArgs e:
            {
                if (e.Source is not Visual || parameter is not Visual visual)
                {
                    return AvaloniaProperty.UnsetValue;
                }
                
                var (x, y) = e.GetPosition(Dispatcher.UIThread.Invoke(() => visual));

                return (x, y);
            }
            case PointerEventArgs e:
            {
                if (e.Source is not Visual || parameter is not Visual visual)
                {
                    return AvaloniaProperty.UnsetValue;
                }
                
                var (x, y) = e.GetPosition(Dispatcher.UIThread.Invoke(() => visual));
                
                return (x, y);
            }
            default:
                return AvaloniaProperty.UnsetValue;
        }
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}