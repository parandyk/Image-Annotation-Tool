using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ImageAnnotationTool.Views;

public partial class MainView : UserControl
{

    public MainView()
    {
        InitializeComponent();
        this.SizeChanged += (_, __) => CloseAllDropdowns();
        // this.PointerReleased += (s, e) => CloseAllDropdowns();
    }

    private void CloseAllDropdowns()
    {
        foreach (var combo in this.GetVisualDescendants().OfType<ComboBox>())
        {
            Dispatcher.UIThread.InvokeAsync(() => 
                combo.IsDropDownOpen = false);
            // combo.IsDropDownOpen = false;
        }
        
        foreach (var ac in this.GetVisualDescendants().OfType<AutoCompleteBox>())
        {
            Dispatcher.UIThread.InvokeAsync(() => 
                ac.IsDropDownOpen = false);
            // combo.IsDropDownOpen = false;
        }
    }
}