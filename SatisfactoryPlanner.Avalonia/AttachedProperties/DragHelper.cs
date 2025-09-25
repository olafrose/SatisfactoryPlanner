using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using SatisfactoryPlanner.Avalonia.Services;

namespace SatisfactoryPlanner.Avalonia.AttachedProperties;

public static class DragHelper
{
    public static readonly AttachedProperty<bool> IsDraggableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsDraggable", typeof(DragHelper), false);

    private static readonly AttachedProperty<bool> IsDraggingProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsDragging", typeof(DragHelper), false);

    private static readonly AttachedProperty<Point> DragStartPointProperty =
        AvaloniaProperty.RegisterAttached<Control, Point>("DragStartPoint", typeof(DragHelper));

    private static readonly AttachedProperty<Point> OriginalPositionProperty =
        AvaloniaProperty.RegisterAttached<Control, Point>("OriginalPosition", typeof(DragHelper));

    public static bool GetIsDraggable(Control control) => control.GetValue(IsDraggableProperty);
    public static void SetIsDraggable(Control control, bool value) => control.SetValue(IsDraggableProperty, value);

    static DragHelper()
    {
        DebugService.Instance.LogDebug("DragHelper static constructor called");
        IsDraggableProperty.Changed.AddClassHandler<Control>(OnIsDraggableChanged);
    }

    private static void OnIsDraggableChanged(Control control, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is bool isDraggable)
        {
            if (isDraggable)
            {
                DebugService.Instance.LogDebug($"Attaching drag to {control.GetType().Name}");
                control.PointerPressed += OnPointerPressed;
                control.PointerMoved += OnPointerMoved;
                control.PointerReleased += OnPointerReleased;
                control.PointerCaptureLost += OnPointerCaptureLost;
                control.Cursor = new Cursor(StandardCursorType.Hand);
                
                // Drag attachment successful
                DebugService.Instance.LogDebug($"Drag functionality attached to {control.GetType().Name}");
            }
            else
            {
                control.PointerPressed -= OnPointerPressed;
                control.PointerMoved -= OnPointerMoved;
                control.PointerReleased -= OnPointerReleased;
                control.PointerCaptureLost -= OnPointerCaptureLost;
            }
        }
    }

    private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control)
        {
            DebugService.Instance.LogDebug("Pointer pressed on draggable control!");
            var properties = e.GetCurrentPoint(control).Properties;
            if (properties.IsLeftButtonPressed)
            {
                DebugService.Instance.LogDebug("Starting drag operation");
                control.SetValue(IsDraggingProperty, true);
                
                var parent = control.Parent as Visual ?? control;
                var startPoint = e.GetPosition(parent);
                control.SetValue(DragStartPointProperty, startPoint);
                
                // Get current position from ViewModel instead of Canvas properties
                var originalPosition = GetViewModelPosition(control);
                control.SetValue(OriginalPositionProperty, originalPosition);
                DebugService.Instance.LogDebug($"Starting drag from ViewModel position: X={originalPosition.X:F1}, Y={originalPosition.Y:F1}");

                e.Pointer?.Capture(control);
                e.Handled = true;
            }
        }
    }

    private static void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Control control && control.GetValue(IsDraggingProperty))
        {
            var parent = control.Parent as Visual ?? control;
            var currentPoint = e.GetPosition(parent);
            var startPoint = control.GetValue(DragStartPointProperty);
            var originalPosition = control.GetValue(OriginalPositionProperty);

            var deltaX = currentPoint.X - startPoint.X;
            var deltaY = currentPoint.Y - startPoint.Y;

            var newX = originalPosition.X + deltaX;
            var newY = originalPosition.Y + deltaY;

            // Keep within bounds
            newX = Math.Max(0, newX);
            newY = Math.Max(0, newY);

            // Update ViewModel and let RenderTransform binding handle the visual update
            UpdateViewModelPosition(control, newX, newY);

            e.Handled = true;
        }
    }

    private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && control.GetValue(IsDraggingProperty))
        {
            control.SetValue(IsDraggingProperty, false);
            e.Pointer?.Capture(null);
            e.Handled = true;
        }
    }

    private static void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (sender is Control control)
        {
            control.SetValue(IsDraggingProperty, false);
        }
    }

    private static Point GetViewModelPosition(Control control)
    {
        if (control.DataContext == null) 
        {
            return new Point(0, 0);
        }

        var dataContext = control.DataContext;
        var type = dataContext.GetType();

        // Use reflection to read XPosition and YPosition properties
        var xProperty = type.GetProperty("XPosition");
        var yProperty = type.GetProperty("YPosition");

        var x = xProperty?.GetValue(dataContext) as double? ?? 0;
        var y = yProperty?.GetValue(dataContext) as double? ?? 0;

        return new Point(x, y);
    }

    private static void UpdateViewModelPosition(Control control, double x, double y)
    {
        if (control.DataContext == null) 
        {
            DebugService.Instance.LogDebug("No DataContext found for position update");
            return;
        }

        // Ensure we're on the UI thread for property updates
        Dispatcher.UIThread.Post(() =>
        {
            var dataContext = control.DataContext;
            var type = dataContext.GetType();

            // Use reflection to update XPosition and YPosition properties
            var xProperty = type.GetProperty("XPosition");
            var yProperty = type.GetProperty("YPosition");

            if (xProperty != null && xProperty.CanWrite)
            {
                xProperty.SetValue(dataContext, x);
            }

            if (yProperty != null && yProperty.CanWrite)
            {
                yProperty.SetValue(dataContext, y);
            }
        });
    }
}