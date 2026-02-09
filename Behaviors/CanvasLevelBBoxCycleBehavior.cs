using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using ImageAnnotationTool.Messages;
using ImageAnnotationTool.Services;
using ImageAnnotationTool.ViewModels;

namespace ImageAnnotationTool.Behaviors;

public class CanvasLevelBBoxCycleBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="Command"/> avalonia property. Used for setting 
    /// </summary>
    public static readonly StyledProperty<ICommand?> CycleCommandProperty =
        AvaloniaProperty.Register<CanvasLevelBBoxCycleBehavior, ICommand?>(nameof(CycleCommand));
    /// <summary>
    /// Gets or sets the command to invoke when cycling
    /// </summary>
    public ICommand? CycleCommand
    {
        get => GetValue(CycleCommandProperty);
        set => SetValue(CycleCommandProperty, value);
    }
    
    private bool _captured;
    
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AppMessengerService.Instance.Register<AbortOperationMessage>(this, OnAbortRequested);
        
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Bubble);
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
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, CaptureLost);
        }
    }
    
    private void OnAbortRequested(object recipient, AbortOperationMessage message)
    {
        _captured = false;
    }
    
    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled) return;
        
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed 
            && IsEnabled)
        {
            _captured = true;
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _captured = false;
    }
    
    private void Released(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                var point = e.GetPosition(AssociatedObject);
                SelectBBox(point);
            }

            _captured = false;
        }
    }

    private void SelectBBox(Point point)
    {
        var overlappingBBoxesList = PerformHitTest(point);
        if (CycleCommand != null && CycleCommand.CanExecute(overlappingBBoxesList))
            CycleCommand.Execute(overlappingBBoxesList);
    }
    
    private List<Guid>? PerformHitTest(Point point)
    {
        if (AssociatedObject is null)
            return null;
        
        var visuals = AssociatedObject.GetVisualsAt(point);
        
        var bboxListGUID = visuals
            .Select(v => v.DataContext)
            .OfType<AnnotationViewModel>()
            .Distinct()
            .OrderByDescending(vm => vm.GUID)
            .Select(vm => vm.GUID)
            .ToList();
        
        return bboxListGUID;
    }
}