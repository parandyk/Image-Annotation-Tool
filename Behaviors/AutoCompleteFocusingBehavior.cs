using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using FluentAvalonia.Core;

namespace ImageAnnotationTool.Behaviors;

public sealed class AutoCompleteFocusingBehavior : Behavior<AutoCompleteBox>
{
    private TopLevel? _topLevel;

    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject is null)
            return;
        
        AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        AssociatedObject.DetachedFromVisualTree += OnDetachedFromVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _topLevel = TopLevel.GetTopLevel(AssociatedObject);
        if (_topLevel == null)
            return;

        _topLevel.AddHandler(InputElement.PointerPressedEvent, OnTopLevelPointerPressed, 
            RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_topLevel != null)
        {
            _topLevel.RemoveHandler(InputElement.PointerPressedEvent, OnTopLevelPointerPressed);
            _topLevel = null;
        }
    }

    private void OnTopLevelPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is null)
            return;
        
        if (!AssociatedObject.IsKeyboardFocusWithin && !AssociatedObject.IsDropDownOpen)
            return;
        
        if (e.Source is not Visual source)
            return;
        
        var pointerPosition = e.GetPosition(_topLevel);

        if (source is LightDismissOverlayLayer)
        {
            if (_topLevel?.GetInputElementsAt(pointerPosition)
                    .FirstOrDefault(ie => ie is not LightDismissOverlayLayer) is Visual substituteVisual)
                source = substituteVisual;
        }
        
        if (AssociatedObject.IsVisualAncestorOf(source))
            return;
        
        if (IsInsidePopup(source))
            return;
        
        ChangeFocus(pointerPosition);
        // _topLevel?.FocusManager?.ClearFocus();
    }

    private void ChangeFocus(Point pointerPosition)
    {
        var inputElements = _topLevel?.GetInputElementsAt(pointerPosition);
        var firstElement = inputElements?.FirstOrDefault(v => v.Focusable);
        firstElement?.Focus();
    }

    private bool IsInsidePopup(Visual source)
    {
        return source.FindAncestorOfType<ListBoxItem>() != null;
    }

    protected override void OnDetaching()
    {
        OnDetachedFromVisualTree(null!, null!);
        base.OnDetaching();
    }
}