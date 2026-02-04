using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImageAnnotationTool.Views;

public partial class SidebarView : UserControl
{
    public SidebarView()
    {
        InitializeComponent();
        
        // if (BorderThicknessNumeric != null)
        // {
        //     BorderThicknessNumeric.Value = (decimal)BboxBorderThickness.GetValueOrDefault();
        // }
    }
    
    // public static readonly StyledProperty<double> BboxBorderThicknessProperty =
    //     AvaloniaProperty.Register<SidebarView, double>(nameof(BboxBorderThickness), defaultValue: 1.0);
            // ,coerce: CoerceThickness
           
    
    // private double _lastValue;
    //
    // private static double CoerceThickness(AvaloniaObject sender, double value)
    // {
    //     if (value < 0 || value > 5 || double.IsNaN(value)) 
    //     {
    //         // return _lastValue; 
    //         return BboxBorderThicknessProperty.GetDefaultValue(sender); 
    //     }
    //     
    //     return value;
    // }
    
    // public double BboxBorderThickness
    // {
    //     get => GetValue(BboxBorderThicknessProperty);
    //     set
    //     {
    //         // if (double.IsNaN(value))
    //         //     return;
    //         //
    //         SetValue(BboxBorderThicknessProperty, value);
    //     }
    // }
}