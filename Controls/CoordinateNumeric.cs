using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactivity;

namespace ImageAnnotationTool.Controls;

public class CoordinateNumeric : NumericUpDown
{
    public static readonly StyledProperty<bool> EnableCommandProperty =
        AvaloniaProperty.Register<CoordinateNumeric, bool>(nameof(EnableCommand), false);
    
    public bool EnableCommand
    {
        get => GetValue(EnableCommandProperty);
        set => SetValue(EnableCommandProperty, value);
    }
    
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<ResizingThumb, ICommand?>(nameof(Command));
    
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var behavior = new EventTriggerBehavior()
        {
            EventName = "ValueChanged"
        };

        var action = new InvokeCommandAction();
        
        action.Bind(InvokeCommandActionBase.CommandProperty, new Binding("Command") 
        { 
            Source = this 
        });
        
        behavior.Actions?.Add(action);
        Interaction.GetBehaviors(this).Add(behavior);
    }
}