using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Threading;


namespace ImageAnnotationTool.ValueConverters;

// public class ZoomLevelsFetchConverter : IMultiValueConverter
// {
//     public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
//     {
//         if (values is null 
//             || values.Any(v => v is UnsetValueType) 
//             || values.Count < 2 
//             || values[0] is not double zoomX 
//             || values[1] is not double zoomY) 
//             return BindingOperations.DoNothing;
//
//         return new ValueTuple<double, double>(zoomX, zoomY);
//     }
// }