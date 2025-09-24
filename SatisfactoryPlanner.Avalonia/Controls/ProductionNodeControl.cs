using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace SatisfactoryPlanner.Avalonia.Controls;

public class ProductionNodeControl : UserControl
{
    public static readonly StyledProperty<string> NodeNameProperty =
        AvaloniaProperty.Register<ProductionNodeControl, string>(nameof(NodeName), string.Empty);

    public static readonly StyledProperty<string> BuildingNameProperty =
        AvaloniaProperty.Register<ProductionNodeControl, string>(nameof(BuildingName), string.Empty);

    public static readonly StyledProperty<double> BuildingCountProperty =
        AvaloniaProperty.Register<ProductionNodeControl, double>(nameof(BuildingCount), 0.0);

    public static readonly StyledProperty<double> ProductionRateProperty =
        AvaloniaProperty.Register<ProductionNodeControl, double>(nameof(ProductionRate), 0.0);

    public static readonly StyledProperty<IImage?> BuildingIconProperty =
        AvaloniaProperty.Register<ProductionNodeControl, IImage?>(nameof(BuildingIcon));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<ProductionNodeControl, bool>(nameof(IsSelected));

    public string NodeName
    {
        get => GetValue(NodeNameProperty);
        set => SetValue(NodeNameProperty, value);
    }

    public string BuildingName
    {
        get => GetValue(BuildingNameProperty);
        set => SetValue(BuildingNameProperty, value);
    }

    public double BuildingCount
    {
        get => GetValue(BuildingCountProperty);
        set => SetValue(BuildingCountProperty, value);
    }

    public double ProductionRate
    {
        get => GetValue(ProductionRateProperty);
        set => SetValue(ProductionRateProperty, value);
    }

    public IImage? BuildingIcon
    {
        get => GetValue(BuildingIconProperty);
        set => SetValue(BuildingIconProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private Border? _mainBorder;

    public ProductionNodeControl()
    {
        InitializeComponent();
        PointerEntered += OnPointerEntered;
        PointerExited += OnPointerExited;
    }

    private void InitializeComponent()
    {
        Width = 120;
        Height = 100;
        Cursor = new Cursor(StandardCursorType.Hand);
        
        Content = CreateNodeContent();
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (_mainBorder != null)
        {
            _mainBorder.BorderBrush = Brushes.DodgerBlue;
            _mainBorder.BorderThickness = new Thickness(2);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (_mainBorder != null)
        {
            _mainBorder.BorderBrush = Brushes.LightGray;
            _mainBorder.BorderThickness = new Thickness(1);
        }
    }

    private Control CreateNodeContent()
    {
        _mainBorder = new Border
        {
            Background = Brushes.White,
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(8),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                OffsetX = 2,
                OffsetY = 2,
                Blur = 4,
                Color = Colors.Black
            })
        };

        var stackPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Fallback icon (we'll make this simple for now)
        var iconBorder = new Border
        {
            Width = 32,
            Height = 32,
            Background = Brushes.LightBlue,
            CornerRadius = new CornerRadius(4),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4),
            Child = new TextBlock
            {
                Text = "🏭",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        // Product name
        var nameBlock = new TextBlock
        {
            FontSize = 11,
            FontWeight = FontWeight.Medium,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            MaxLines = 2,
            Margin = new Thickness(0, 0, 0, 2)
        };
        nameBlock.Text = NodeName;

        // Building info
        var buildingInfoPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var countBlock = new TextBlock
        {
            Text = $"{BuildingCount:F1}x",
            FontSize = 9,
            Foreground = Brushes.Gray
        };

        var buildingBlock = new TextBlock
        {
            Text = BuildingName,
            FontSize = 9,
            Foreground = Brushes.Gray,
            Margin = new Thickness(2, 0, 0, 0)
        };

        buildingInfoPanel.Children.Add(countBlock);
        buildingInfoPanel.Children.Add(buildingBlock);

        // Production rate
        var rateBlock = new TextBlock
        {
            Text = $"{ProductionRate:F1}/min",
            FontSize = 9,
            Foreground = Brushes.DarkGray,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 1, 0, 0)
        };

        stackPanel.Children.Add(iconBorder);
        stackPanel.Children.Add(nameBlock);
        stackPanel.Children.Add(buildingInfoPanel);
        stackPanel.Children.Add(rateBlock);

        _mainBorder.Child = stackPanel;

        return _mainBorder;
    }
}