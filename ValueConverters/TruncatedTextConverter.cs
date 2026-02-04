using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Xaml.Interactions.Custom;
using ImageAnnotationTool.Enums;
using Microsoft;

namespace ImageAnnotationTool.ValueConverters;

public class TruncatedTextConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Any(v => v is null))
        { 
            return BindingOperations.DoNothing;
        }

        if (values.Count < 4 ||
            !(values[0] is string text) ||
            !(values[1] is TruncationMode mode) ||
            !(values[2] is string length) ||
            !(values[3] is bool hasExtension))
        {
            return AvaloniaProperty.UnsetValue;
        }
        
        if (!UInt16.TryParse(length, out ushort intLength))
        {
            return BindingOperations.DoNothing;
        }
        
        if (text.Length <= intLength)
            return text;

        const string separator = "…";

        var extension = string.Empty;
        
        if (hasExtension)
        {
            if (text.Contains('.'))
                extension = text[text.LastIndexOf('.')..];
        }
        
        switch (mode)
        {
            case TruncationMode.None: return text;
            case TruncationMode.Start:
            {
                var truncatedText = separator.Concat(text.Substring(Math.Max(0, text.Length - intLength - extension.Length)));
                return new string(truncatedText.ToArray());
            }
            case TruncationMode.End:
            {
                var truncatedText = text.Substring(0, intLength).Concat(separator);
                return new string(truncatedText.ToArray());
            }
            case TruncationMode.Middle:
            {
                var remainderLength = text.Length - intLength - extension.Length;
                if (remainderLength <= intLength)
                {
                    var truncatedTextReplacement = text.Substring(0, intLength).Concat(separator);
                    return new string(truncatedTextReplacement.ToArray());
                }
                
                var beforeSeparator = text.Substring(0, intLength);
                var pastSeparator = text.Substring(Math.Max(0, remainderLength));
                var truncatedText = beforeSeparator.Concat(separator).Concat(pastSeparator);
                
                return new string(truncatedText.ToArray());
            }
            default:
                return text;
        }
    }
}