using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace SatisfactoryPlanner.Avalonia.Controls;

public class ProductionFlowCanvas : Canvas
{
    private ProductionNodeControl? _draggedNode;
    private Point _dragStartPoint;
    private Point _originalNodePosition;
    private readonly List<FlowConnection> _connections = new();

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        
        // Find if we clicked on a node
        var position = e.GetPosition(this);
        foreach (var child in Children)
        {
            if (child is ProductionNodeControl node)
            {
                var nodeLeft = Canvas.GetLeft(node);
                var nodeTop = Canvas.GetTop(node);
                // Use the actual Width and Height properties instead of Bounds
                var nodeRect = new Rect(nodeLeft, nodeTop, node.Width, node.Height);
                
                if (nodeRect.Contains(position))
                {
                    _draggedNode = node;
                    _dragStartPoint = position;
                    _originalNodePosition = new Point(Canvas.GetLeft(node), Canvas.GetTop(node));
                    e.Pointer?.Capture(this); // Capture pointer for smoother dragging
                    e.Handled = true;
                    break;
                }
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        
        if (_draggedNode != null)
        {
            var currentPoint = e.GetPosition(this);
            
            // Calculate position directly from the original drag start point
            // to avoid accumulating drift from incremental updates
            var totalOffset = currentPoint - _dragStartPoint;
            var originalLeft = _originalNodePosition.X;
            var originalTop = _originalNodePosition.Y;
            
            var newLeft = originalLeft + totalOffset.X;
            var newTop = originalTop + totalOffset.Y;
            
            // Ensure the node stays within bounds
            newLeft = Math.Max(0, Math.Min(newLeft, Bounds.Width - _draggedNode.Width));
            newTop = Math.Max(0, Math.Min(newTop, Bounds.Height - _draggedNode.Height));
            
            Canvas.SetLeft(_draggedNode, newLeft);
            Canvas.SetTop(_draggedNode, newTop);
            
            // Don't update connection visuals during drag - too expensive
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_draggedNode != null)
        {
            e.Pointer?.Capture(null); // Release pointer capture
            UpdateConnectionVisuals(); // Final update of connection lines
            _draggedNode = null;
        }
    }

    public void AddConnection(ProductionNodeControl from, ProductionNodeControl to)
    {
        _connections.Add(new FlowConnection(from, to));
        UpdateConnectionVisuals();
    }

    public void RemoveConnection(ProductionNodeControl from, ProductionNodeControl to)
    {
        _connections.RemoveAll(c => c.From == from && c.To == to);
        UpdateConnectionVisuals();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);
        UpdateConnectionVisuals(); // Update connections when layout changes
        return result;
    }

    private void UpdateConnectionVisuals()
    {
        // Find existing connection lines and update them in place if possible
        var existingLines = Children.OfType<Line>().ToList();
        var connectionIndex = 0;

        foreach (var connection in _connections)
        {
            var fromPoint = GetNodeConnectionPoint(connection.From, true);
            var toPoint = GetNodeConnectionPoint(connection.To, false);
            
            Line line;
            if (connectionIndex < existingLines.Count)
            {
                // Reuse existing line
                line = existingLines[connectionIndex];
                line.StartPoint = fromPoint;
                line.EndPoint = toPoint;
            }
            else
            {
                // Create new line
                line = new Line
                {
                    StartPoint = fromPoint,
                    EndPoint = toPoint,
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 2,
                    ZIndex = -1 // Behind the nodes
                };
                Children.Add(line);
            }
            connectionIndex++;
        }

        // Remove any excess lines
        for (int i = connectionIndex; i < existingLines.Count; i++)
        {
            Children.Remove(existingLines[i]);
        }
    }

    public void AddProductionNode(ProductionNodeControl node, double x, double y)
    {
        Children.Add(node);
        Canvas.SetLeft(node, x);
        Canvas.SetTop(node, y);
    }

    private Point GetNodeConnectionPoint(ProductionNodeControl node, bool isOutput)
    {
        var nodeLeft = Canvas.GetLeft(node);
        var nodeTop = Canvas.GetTop(node);
        var nodeWidth = node.Width;
        var nodeHeight = node.Height;
        
        if (isOutput)
        {
            // Right side connection point
            return new Point(nodeLeft + nodeWidth, nodeTop + nodeHeight / 2);
        }
        else
        {
            // Left side connection point
            return new Point(nodeLeft, nodeTop + nodeHeight / 2);
        }
    }
}

public class FlowConnection
{
    public ProductionNodeControl From { get; }
    public ProductionNodeControl To { get; }
    
    public FlowConnection(ProductionNodeControl from, ProductionNodeControl to)
    {
        From = from;
        To = to;
    }
}