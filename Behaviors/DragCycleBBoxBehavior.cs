using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Draggable;
using Avalonia.Xaml.Interactions.Events;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using ImageAnnotationTool.Interfaces;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Services;
using ImageAnnotationTool.ViewModels;
using ImageAnnotationTool.Core;
using ImageAnnotationTool.Models;

namespace ImageAnnotationTool.Behaviors;

public class DragCycleBBoxBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="IsAnchored"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAnchoredProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, bool>(nameof(IsAnchored));
    
    /// <summary>
    /// Identifies the <see cref="ConstrainToParentBounds"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> ConstrainToParentBoundsProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, bool>(nameof(ConstrainToParentBounds));
    
    /// <summary>
    /// Identifies the <see cref="CycleCommand"/> avalonia property. Used for cycling between overlapping boxes
    /// </summary>
    public static readonly StyledProperty<ICommand?> CycleCommandProperty =
        AvaloniaProperty.Register<DragCycleBBoxBehavior, ICommand?>(nameof(CycleCommand));
    
    /// <summary>
    /// Identifies the <see cref="DragCommand"/> avalonia property. Used for dragging the box 
    /// </summary>
    public static readonly StyledProperty<ICommand?> DragCommandProperty =
        AvaloniaProperty.Register<DragCycleBBoxBehavior, ICommand?>(nameof(DragCommand));
    
    /// <summary>
    /// Identifies the <see cref="CancelCommand"/> avalonia property. Used for cancelling the dragging operation 
    /// </summary>
    public static readonly StyledProperty<ICommand?> CancelCommandProperty =
        AvaloniaProperty.Register<DragCycleBBoxBehavior, ICommand?>(nameof(CancelCommand));
    
    /// <summary>
    /// Identifies the <see cref="ChangeDragStateCommand"/> avalonia property. Used for interpreting the state of dragging
    /// </summary>
    public static readonly StyledProperty<ICommand?> ChangeDragStateCommandProperty =
        AvaloniaProperty.Register<DragCycleBBoxBehavior, ICommand?>(nameof(ChangeDragStateCommand));
    
    // /// <summary>
    // /// Identifies the <see cref="DragThreshold"/> avalonia property. Used for setting how much mouse movement needs to happen before dragging starts 
    // /// </summary>
    public static readonly StyledProperty<double> DragThresholdProperty =
        AvaloniaProperty.Register<DragCycleBBoxBehavior, double>(nameof(DragThreshold), InteractionConstants.DefaultDragThreshold);
    
    /// <summary>
    /// Gets or sets whether control is anchored (ineligible for dragging).
    /// </summary>
    public bool IsAnchored
    {
        get => GetValue(IsAnchoredProperty);
        set => SetValue(IsAnchoredProperty, value);
    }
    
    /// <summary>
    /// Gets or sets whether dragging should be constrained to the bounds of the parent control.
    /// </summary>
    public bool ConstrainToParentBounds
    {
        get => GetValue(ConstrainToParentBoundsProperty);
        set => SetValue(ConstrainToParentBoundsProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the command to invoke when cycling
    /// </summary>
    public ICommand? CycleCommand
    {
        get => GetValue(CycleCommandProperty);
        set => SetValue(CycleCommandProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the command to invoke when dragging
    /// </summary>
    public ICommand? DragCommand
    {
        get => GetValue(DragCommandProperty);
        set => SetValue(DragCommandProperty, value);
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
    /// Gets or sets the command to invoke when dragging state changes
    /// </summary>
    public ICommand? ChangeDragStateCommand
    {
        get => GetValue(ChangeDragStateCommandProperty);
        set => SetValue(ChangeDragStateCommandProperty, value);
    }
    
    /// <summary>
    /// Gets or sets drag hysteresis threshold.
    /// </summary>
    public double DragThreshold
    {
        get => GetValue(DragThresholdProperty);
        set => SetValue(DragThresholdProperty, value);
    }

    private Control? _resizableHost;
    private Control? _itemHost;
    private Control? _parent;
    private Control? _draggedContainer;
    private Control? _adorner;
    private Point _start;
    
    private bool _isPending;
    private bool _captured;
    private bool _enableDrag;
    
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AppMessengerService.Instance.Register<AbortOperationMessage>(this, OnAbortRequested);
        
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, Released);
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, Moved);
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, CaptureLost);
        }
    }
    
    private void OnAbortRequested(object recipient, AbortOperationMessage message)
    {
        var behavior = (DragCycleBBoxBehavior)recipient;
        CancelCommand?.Execute(null);
        behavior.Released();
    }

    private void AddAdorner(Control control)
    {
        var layer = AdornerLayer.GetAdornerLayer(control);
        if (layer is null)
        {
            return;
        }

        _adorner = new SelectionAdorner()
        {
            [AdornerLayer.AdornedElementProperty] = control
        };

        ((ISetLogicalParent) _adorner).SetParent(control);
        layer.Children.Add(_adorner);
    }

    private void RemoveAdorner(Control control)
    {
        var layer = AdornerLayer.GetAdornerLayer(control);
        if (layer is null || _adorner is null)
        {
            return;
        }

        layer.Children.Remove(_adorner);
        ((ISetLogicalParent) _adorner).SetParent(null);
        _adorner = null;
    }
    
    private void Pressed(object? sender, PointerPressedEventArgs e) //TODO!!!
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed
            && AssociatedObject?.Parent is Control parent
            && AssociatedObject.DataContext is IInteractiveElement
            && IsEnabled)
        {
            _isPending = true;
            _start = e.GetPosition(parent);
            _parent = parent;
            _itemHost = AssociatedObject.GetVisualAncestors().OfType<Grid>().Skip(1).FirstOrDefault();
            _resizableHost = AssociatedObject.GetVisualAncestors().OfType<Viewbox>().FirstOrDefault();

            _enableDrag = false;
            _captured = true;
            e.Handled = true;
        }
    }

    private void Released(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                _start = e.GetPosition(_itemHost);
                
                if (!_enableDrag)
                    Cycle();
                
                Released();
            }
            // _captured = false;
            e.Handled = true;
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Released();
        // _captured = false;
    }
    
    private void Moved(object? sender, PointerEventArgs e)
    {
        if (!IsEnabled)
            return;

        if (_enableDrag)
        {
            Drag(e);
            return;
        }
        
        if (!_isPending)
            return;
        
        var newPoint = e.GetPosition(_parent);
        
        var moved = VerifyMovement(_start, newPoint);

        if (!moved)
            return;
        
        _isPending = false;
        
        _enableDrag = true;
        _start = newPoint;
        
        if (ChangeDragStateCommand != null && ChangeDragStateCommand.CanExecute(_enableDrag))
            ChangeDragStateCommand.Execute(_enableDrag);
        
        _draggedContainer = AssociatedObject;
        _draggedContainer.Cursor = new Cursor(StandardCursorType.DragMove);
        
        SetDraggingPseudoClasses(_draggedContainer, true);
        
        Drag(e);
    }

    private void Released()
    {
        if (_enableDrag)
        {
            _enableDrag = false;
            if (ChangeDragStateCommand != null && ChangeDragStateCommand.CanExecute(_enableDrag))
                ChangeDragStateCommand.Execute(_enableDrag);
        }
        
        if (_itemHost is not null && _parent is not null && _draggedContainer is not null)
        {
            // RemoveAdorner(_draggedContainer);
            _itemHost = null;
            SetDraggingPseudoClasses(_draggedContainer, false);
            _draggedContainer.Cursor = new Cursor(StandardCursorType.Hand);
            _draggedContainer = null;
            _parent = null;
        }
        
        _captured = false; 
        _isPending = false;
    }

    private void SetDraggingPseudoClasses(Control control, bool isDragging)
    {
        if (isDragging)
        {
            ((IPseudoClasses)control.Classes).Add(":dragging");
        }
        // else if (isResizing)
        // {
        //     ((IPseudoClasses)control.Classes).Add(":resizing");
        // }
        else
        {
            ((IPseudoClasses)control.Classes).Remove(":dragging");
        }
    }
    
    private void Drag(PointerEventArgs e)
    {
        if (IsAnchored)
            return;
        
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (_captured && properties.IsLeftButtonPressed && IsEnabled
            && AssociatedObject.DataContext is IInteractiveElement bbox)
        {
            if (_parent is null || _draggedContainer is null || !_enableDrag)
            {
                return;
            }

            bbox.IsSelected = true;
            var position = e.GetPosition(_parent);
            var deltaX = position.X - _start.X;
            var deltaY = position.Y - _start.Y;
            _start = position;

            var left = bbox.X;
            var top = bbox.Y;
            
            var newLeft = left + deltaX;
            var newTop = top + deltaY;
            
            if (ConstrainToParentBounds)
            {
                var canvasWidth = _parent.Bounds.Width;
                var canvasHeight = _parent.Bounds.Height;
                
                var containerWidth = bbox.Width;
                var containerHeight = bbox.Height;
            
                newLeft = Math.Clamp(newLeft, 0, canvasWidth - containerWidth);
                newTop = Math.Clamp(newTop, 0, canvasHeight - containerHeight);
            }

            var newPosition = new PointerPositionModel
            {
                PX = newLeft,
                PY = newTop
            };
            
            if (DragCommand != null && DragCommand.CanExecute(newPosition))
                DragCommand.Execute(newPosition);
            else
            {
                bbox.X = newPosition.PX;
                bbox.Y = newPosition.PY;
            }
        }
    }

    private void Cycle()
    {
        var overlappingBBoxesList = PerformHitTest(_start);
        if (CycleCommand != null && CycleCommand.CanExecute(overlappingBBoxesList))
            CycleCommand.Execute(overlappingBBoxesList);
    }
    
    private List<Guid>? PerformHitTest(Point point)
    {
        if (_itemHost is null)
            return null;
        
        var visuals = _itemHost.GetVisualsAt(point);
        
        var bboxListGUID = visuals
            .Select(v => v.DataContext)
            .OfType<AnnotationViewModel>()
            .Distinct()
            .OrderByDescending(vm => vm.GUID)
            .Select(vm => vm.GUID)
            .ToList();
        
        return bboxListGUID;
    }

    private bool VerifyMovement(Point start, Point end)
    {
        double adjustedDragThresholdX = DragThreshold;
        double adjustedDragThresholdY = DragThreshold;
        
        if (_resizableHost != null && _itemHost != null)
        {
            adjustedDragThresholdX = DragThreshold * _resizableHost.Bounds.Width / _itemHost.Bounds.Width;
            adjustedDragThresholdY = DragThreshold * _resizableHost.Bounds.Height / _itemHost.Bounds.Height;
        }
        
        var moved = Math.Abs(start.X - end.X) > adjustedDragThresholdX 
                    || Math.Abs(start.Y - end.Y) > adjustedDragThresholdY;
        
        Debug.WriteLine($"drag threshold: {DragThreshold}");
        Debug.WriteLine($"Adjusted X drag threshold: {adjustedDragThresholdX}");
        Debug.WriteLine($"Adjusted Y drag threshold: {adjustedDragThresholdY}");
        return moved;
    }
}