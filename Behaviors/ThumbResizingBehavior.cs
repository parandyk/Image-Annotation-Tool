using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Draggable;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using ExCSS;
using ImageAnnotationTool.Controls;
using ImageAnnotationTool.Domain.ValueObjects;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Models;
using ImageAnnotationTool.Services;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using Point = Avalonia.Point;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace ImageAnnotationTool.Behaviors;

public class ThumbResizingBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="ChangeResizeStateCommand"/> avalonia property. Used for interpreting the state of resizing
    /// </summary>
    public static readonly StyledProperty<ICommand?> ChangeResizeStateCommandProperty =
        AvaloniaProperty.Register<ThumbResizingBehavior, ICommand?>(nameof(ChangeResizeStateCommand));
    
    /// <summary>
    /// Identifies the <see cref="CancelCommand"/> avalonia property. Used for cancelling the resizing operation 
    /// </summary>
    public static readonly StyledProperty<ICommand?> CancelCommandProperty =
        AvaloniaProperty.Register<ThumbResizingBehavior, ICommand?>(nameof(CancelCommand));
    
    /// <summary>
    /// Identifies the <see cref="ResizeProperty"/> avalonia property. Used for resizing the control
    /// </summary>
    public static readonly StyledProperty<ICommand?> ResizeCommandProperty =
        AvaloniaProperty.Register<ThumbResizingBehavior, ICommand?>(nameof(ResizeCommand), null);
    
    /// <summary>
    /// Identifies the <see cref="ConstrainToParentBounds"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> ConstrainToParentBoundsProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, bool>(nameof(ConstrainToParentBounds));
    
    /// <summary>
    /// Identifies the <see cref="TargetParentProperty"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetParentProperty =
        AvaloniaProperty.Register<ThumbResizingBehavior, Control?>(nameof(TargetParent));
    
    private Control? _parent;
    private Control? _draggedThumb;
    private Control? _resizedContainer;
    private Point _start;
    private double? _initialLeftPY;
    private double? _initialLeftPX;
    private double? _initialRightPY;
    private double? _initialRightPX;
    private double _cumulativeDeltaX = 0;
    private double _cumulativeDeltaY = 0;
    private bool _captured;
    private bool _lmbPressed;
    private bool _enableDrag;
    
    public bool ConstrainToParentBounds
    {
        get => GetValue(ConstrainToParentBoundsProperty);
        set => SetValue(ConstrainToParentBoundsProperty, value);
    }
    
    public Control? TargetParent
    {
        get => GetValue(TargetParentProperty);
        set => SetValue(TargetParentProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the command to invoke when resizing state changes
    /// </summary>
    public ICommand? ChangeResizeStateCommand
    {
        get => GetValue(ChangeResizeStateCommandProperty);
        set => SetValue(ChangeResizeStateCommandProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the command to invoke when cancelling
    /// </summary>
    public ICommand? CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the command to invoke when resizing
    /// </summary>
    public ICommand? ResizeCommand
    {
        get => GetValue(ResizeCommandProperty);
        set => SetValue(ResizeCommandProperty, value);
    }
    
    protected override void OnAttachedToVisualTree()
    {
        AppMessengerService.Instance.Register<AbortOperationMessage>(this, OnAbortRequested);
        
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, PointerReleased, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Tunnel);
        }
    }
    
    protected override void OnDetachedFromVisualTree()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, PointerReleased);
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, Moved);
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, CaptureLost);
        }
    }
    
    private void OnAbortRequested(object recipient, AbortOperationMessage message)
    { 
        var behavior = (ThumbResizingBehavior)recipient;
        CancelCommand?.Execute(null);
        behavior.Released();
        _captured = false;
        _lmbPressed = false;;
    }
    
    private void Moved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (_captured && properties.IsLeftButtonPressed && IsEnabled
            && TargetParent?.DataContext is IInteractiveElement bbox)

        {
            if (AssociatedObject is not ResizingThumb thumb)
                return;
            
            // if (thumb.Tag is not string direction)
            //     return;

            var horizontal = thumb.HorizontalAlignment;
            var vertical = thumb.VerticalAlignment;
            
            {
                if (_parent is null
                    || _draggedThumb is null
                    || _resizedContainer is null
                    || !_enableDrag)
                {
                    return;
                }

                bbox.IsSelected = true;
                var position = e.GetPosition(_parent);
                var deltaX = position.X - _start.X;
                var deltaY = position.Y - _start.Y;
                _cumulativeDeltaX += deltaX;
                _cumulativeDeltaY += deltaY;
                _start = position;

                var newLeft = _initialLeftPX;
                var newTop = _initialLeftPY;
                
                var newRight = _initialRightPX;
                var newBottom = _initialRightPY;

                // if (direction.ToLower().Contains("left"))
                if (horizontal == HorizontalAlignment.Left)
                {
                    if (_initialLeftPX + _cumulativeDeltaX > _initialRightPX)
                    {
                        newRight = _initialLeftPX + _cumulativeDeltaX;
                        newLeft = _initialRightPX;
                    }
                    else
                    {
                        newLeft += _cumulativeDeltaX;
                    }
                }
                // else if (direction.ToLower().Contains("right"))
                else if (horizontal == HorizontalAlignment.Right)
                {
                    if (_initialRightPX + _cumulativeDeltaX < _initialLeftPX)
                    {
                        newRight = _initialLeftPX;
                        newLeft = _initialRightPX + _cumulativeDeltaX;
                    }
                    else
                    {
                        newRight += _cumulativeDeltaX;
                    }
                }

                // if (direction.ToLower().Contains("top"))
                if (vertical == VerticalAlignment.Top)
                {
                    if (_initialLeftPY + _cumulativeDeltaY > _initialRightPY)
                    {
                        newBottom = _initialLeftPY + _cumulativeDeltaY;
                        newTop = _initialRightPY;
                    }
                    else
                    {
                        newTop += _cumulativeDeltaY;
                    }
                }
                // else if (direction.ToLower().Contains("bottom"))
                else if (vertical == VerticalAlignment.Bottom)
                {
                    if (_initialRightPY + _cumulativeDeltaY < _initialLeftPY)
                    {
                        newBottom = _initialLeftPY;
                        newTop = _initialRightPY + _cumulativeDeltaY;
                    }
                    else
                    {
                        newBottom += _cumulativeDeltaY;
                    }
                }
                
                var setLeft = newLeft.Value;
                var setTop = newTop.Value;
                var setRight = newRight.Value;
                var setBottom = newBottom.Value;
                
                if (ConstrainToParentBounds)
                {
                    var canvasWidth = _parent.Bounds.Width;
                    var canvasHeight = _parent.Bounds.Height;
                    
                    setLeft = Math.Clamp(setLeft, 0, canvasWidth); 
                    setTop = Math.Clamp(setTop, 0, canvasHeight);

                    setRight = Math.Clamp(setRight, 0, canvasWidth);
                    setBottom = Math.Clamp(setBottom, 0, canvasHeight);
                }
                
                var setWidth = Math.Abs(setLeft - setRight);
                var setHeight = Math.Abs(setTop - setBottom);

                // var newBBox = new BoundingBoxModel
                // {
                //     X1 = setLeft,
                //     Y1 = setTop,
                //     Width = setWidth,
                //     Height = setHeight
                // };
                
                var newBBox = new BoundingBox(
                    setLeft, 
                    setTop, 
                    setLeft + setWidth, 
                    setTop + setHeight);
                
                if (ResizeCommand != null && ResizeCommand.CanExecute(newBBox))
                    ResizeCommand.Execute(newBBox);
                else
                {
                    bbox.X = newBBox.X1;
                    bbox.Y = newBBox.Y1;
                    bbox.Width = newBBox.Width;
                    bbox.Height = newBBox.Height;
                }
                
                // bbox.X = setLeft;
                // bbox.Y = setTop;
                //
                // bbox.Width = setWidth;
                // bbox.Height = setHeight;
            }
        }
    }
        
    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is not ResizingThumb thumb)
            return;
        
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed 
            && TargetParent is Control resizedContainer
            && TargetParent.DataContext is IInteractiveElement bbox
            && TargetParent.Parent is Control parent 
            && IsEnabled)
        {
            // _lmbPressed = true;
            _enableDrag = true;
            _start = e.GetPosition(parent);
            _parent = parent;
            _draggedThumb = AssociatedObject;
            _resizedContainer = resizedContainer;

            bbox.IsSelected = true;
            _initialLeftPX = bbox.X;
            _initialLeftPY = bbox.Y;
            _initialRightPX = _initialLeftPX + bbox.Width;
            _initialRightPY = _initialLeftPY + bbox.Height;
            _draggedThumb.Cursor = new Cursor(StandardCursorType.DragMove);
            
            SetResizingPseudoClasses(_draggedThumb, true);
            SetResizingPseudoClasses(_resizedContainer, true);

            // AddAdorner(_draggedContainer);

            _captured = true;
        }
    }


    private void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                Released();
            }
            
            _captured = false;
            _lmbPressed = false;
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Released();
        _captured = false;
        _lmbPressed = false;
    }

    private void Released()
    {
        if (_enableDrag)
        {
            if (_parent is not null 
                && _resizedContainer is not null 
                && _draggedThumb is not null)
            {
                // RemoveAdorner(_draggedContainer);
            }

            if (_resizedContainer is not null 
                && _draggedThumb is not null)
            {
                SetResizingPseudoClasses(_resizedContainer, false);
                SetResizingPseudoClasses(_draggedThumb, false);
                
                _draggedThumb.Cursor = new Cursor(StandardCursorType.Hand);
                _draggedThumb = null;
            }

            _cumulativeDeltaX = 0;
            _cumulativeDeltaY = 0;
            _initialLeftPY = null;
            _initialLeftPX = null;
            _initialRightPY = null;
            _initialRightPX = null;
            _enableDrag = false;
            _parent = null;
            _resizedContainer = null;
            // _draggedThumb.Cursor = new Cursor(StandardCursorType.Hand);
            // _draggedThumb = null;
        }
    }

    private void SetResizingPseudoClasses(Control control, bool isResizing)
    {
        if (isResizing)
        {
            ((IPseudoClasses)control.Classes).Add(":resizing");
        }
        else
        {
            ((IPseudoClasses)control.Classes).Remove(":resizing");
        }
    }
}