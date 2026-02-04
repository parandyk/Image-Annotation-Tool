using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Domain.Infrastructure;

namespace ImageAnnotationTool.Views;

public partial class WorkspaceView : UserControl, IRenderingInterface
{
    public WorkspaceView()
    {
        InitializeComponent();
        // var img = this.FindControl<Image>("ImgBox");
        // var viewbox = this.FindControl<Viewbox>("ImageViewbox");
        
        if (ImgBox != null && ImageViewbox != null)
        {
            ImgBox.Loaded += (s, e) => UpdateInverseScale();
            ImageViewbox.SizeChanged += (s, e) => UpdateInverseScale();
        }
        
        this.GetObservable(ZoomBorderScaleXProperty).Subscribe(_ => UpdateInverseScale());
        // this.GetObservable(ZoomBorderScaleYProperty).Subscribe(_ => UpdateInverseScale());
    }
    
    public static readonly StyledProperty<bool> DynamicBordersOnProperty =
        AvaloniaProperty.Register<WorkspaceView, bool>(nameof(DynamicBordersOn), false);
    
    public bool DynamicBordersOn
    {
        get => GetValue(DynamicBordersOnProperty);
        set => SetValue(DynamicBordersOnProperty, value);
    }
    
    public static readonly StyledProperty<bool> BBoxBorderOnProperty =
        AvaloniaProperty.Register<WorkspaceView, bool>(nameof(BBoxBorderOn), true);
    
    public bool BBoxBorderOn
    {
        get => GetValue(BBoxBorderOnProperty);
        set => SetValue(BBoxBorderOnProperty, value);
    }
    
    public static readonly StyledProperty<bool> BBoxBackgroundOnProperty =
        AvaloniaProperty.Register<WorkspaceView, bool>(nameof(BBoxBackgroundOn), true);
    
    public bool BBoxBackgroundOn
    {
        get => GetValue(BBoxBackgroundOnProperty);
        set => SetValue(BBoxBackgroundOnProperty, value);
    }
    
    public static readonly StyledProperty<double> DragThresholdProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(DragThreshold), defaultValue: InteractionConstants.DefaultDragThreshold);
    
    public double DragThreshold
    {
        get => GetValue(DragThresholdProperty);
        set => SetValue(DragThresholdProperty, value);
    }
    
    public static readonly StyledProperty<double> BboxBorderThicknessProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(BboxBorderThickness), defaultValue: RenderingConstants.DefaultThickness);
    
    public double BboxBorderThickness
    {
        get => GetValue(BboxBorderThicknessProperty);
        set => SetValue(BboxBorderThicknessProperty, value);
    }

    public static readonly StyledProperty<double> ZoomBorderScaleXProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(ZoomBorderScaleX), defaultValue: 1.0);
    
    public double ZoomBorderScaleX
    {
        get => GetValue(ZoomBorderScaleXProperty);
        set => SetValue(ZoomBorderScaleXProperty, value);
    }
    
    public static readonly StyledProperty<double> ZoomBorderScaleYProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(ZoomBorderScaleY), defaultValue: 1.0);
    
    public double ZoomBorderScaleY
    {
        get => GetValue(ZoomBorderScaleYProperty);
        set => SetValue(ZoomBorderScaleYProperty, value);
    }
    
    public static readonly StyledProperty<double> InverseScaleProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(InverseScale), defaultValue: 1.0);
    
    public double InverseScale
    {
        get => GetValue(InverseScaleProperty);
        set => SetValue(InverseScaleProperty, value);
    }
    
    public static readonly StyledProperty<double> InverseScaleXProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(InverseScaleX), defaultValue: 1.0);
    
    public double InverseScaleX
    {
        get => GetValue(InverseScaleXProperty);
        set => SetValue(InverseScaleXProperty, value);
    }
    
    public static readonly StyledProperty<double> InverseScaleYProperty =
        AvaloniaProperty.Register<WorkspaceView, double>(nameof(InverseScaleY), defaultValue: 1.0);
    
    public double InverseScaleY
    {
        get => GetValue(InverseScaleYProperty);
        set => SetValue(InverseScaleYProperty, value);
    }
    
    private void UpdateInverseScale()
    {
        // var viewbox = this.FindControl<Viewbox>("ImageViewbox");
        // var grid = this.FindControl<Grid>("ImageGrid");

        if (ImgBox == null || ImageViewbox == null)
            return;

        if (ImgBox.Bounds.Width == 0 || 
            ImgBox.Bounds.Height == 0 ||
            ImageViewbox.Bounds.Width == 0 || 
            ImageViewbox.Bounds.Height == 0)
            return;
        
        // double viewboxScaleX = viewbox.Bounds.Width / grid.Width;
        // double viewboxScaleY = viewbox.Bounds.Height / grid.Height;
        
        double viewboxScaleX = ImageViewbox.Bounds.Width / ImgBox.Bounds.Width;
        double viewboxScaleY = ImageViewbox.Bounds.Height / ImgBox.Bounds.Height;
        
        // double viewboxScale = Math.Min(viewboxScaleX, viewboxScaleY);
        // double zoomBorderScale = Math.Min(ZoomBorderScaleX, ZoomBorderScaleY);
        
        // double totalScale = viewboxScale * zoomBorderScale;
        double totalScaleX = viewboxScaleX * ZoomBorderScaleX;
        double totalScaleY = viewboxScaleY * ZoomBorderScaleY;
        
        // InverseScale = 1.0 / totalScale;
        InverseScaleX = 1.0 / totalScaleX;
        InverseScaleY = 1.0 / totalScaleY;
    }
}