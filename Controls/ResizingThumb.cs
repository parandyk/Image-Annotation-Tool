using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactions.Draggable;
using Avalonia.Xaml.Interactivity;
using ImageAnnotationTool.Behaviors;

namespace ImageAnnotationTool.Controls;

public class ResizingThumb : Thumb
{
    public static readonly StyledProperty<Control?> TargetParentProperty =
        AvaloniaProperty.Register<ResizingThumb, Control?>(nameof(TargetParent));

    public static readonly StyledProperty<double> HitTestMarginProperty =
        AvaloniaProperty.Register<ResizingThumb, double>(nameof(HitTestMargin));
    
    public static readonly StyledProperty<bool> ConstrainToParentBoundsProperty =
        AvaloniaProperty.Register<ResizingThumb, bool>(nameof(ConstrainToParentBounds));
    
    public static readonly StyledProperty<ICommand?> ChangeResizeStateCommandProperty =
        AvaloniaProperty.Register<ResizingThumb, ICommand?>(nameof(ChangeResizeStateCommand), null);
    
    public static readonly StyledProperty<ICommand?> ResizeCommandProperty =
        AvaloniaProperty.Register<ResizingThumb, ICommand?>(nameof(ResizeCommand), null);
    
    public static readonly StyledProperty<ICommand?> CancelCommandProperty =
        AvaloniaProperty.Register<ThumbResizingBehavior, ICommand?>(nameof(CancelCommand));
    
    public Control? TargetParent
    {
        get => GetValue(TargetParentProperty);
        set => SetValue(TargetParentProperty, value);
    }

    public double HitTestMargin
    {
        get => GetValue(HitTestMarginProperty);
        set => SetValue(HitTestMarginProperty, value);
    }
    
    public bool ConstrainToParentBounds
    {
        get => GetValue(ConstrainToParentBoundsProperty);
        set => SetValue(ConstrainToParentBoundsProperty, value);
    }
    
    public ICommand? ChangeResizeStateCommand
    {
        get => GetValue(ChangeResizeStateCommandProperty);
        set => SetValue(ChangeResizeStateCommandProperty, value);
    }
    
    public ICommand? ResizeCommand
    {
        get => GetValue(ResizeCommandProperty);
        set => SetValue(ResizeCommandProperty, value);
    }
    
    public ICommand? CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var behavior = new ThumbResizingBehavior
        {
            ConstrainToParentBounds = this.ConstrainToParentBounds,
            TargetParent = this.TargetParent,
            ChangeResizeStateCommand = this.ChangeResizeStateCommand,
            ResizeCommand = this.ResizeCommand,
            CancelCommand = this.CancelCommand,
        };
        Interaction.GetBehaviors(this).Add(behavior);
    }
}