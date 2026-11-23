using ModuleXmlReader.Presentation.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Avalonia;

// View-only navigation: no document mutation or file access.
public sealed class ModuleViewport : UserControl
{
    public static readonly StyledProperty<PreviewData?> DataProperty = AvaloniaProperty.Register<ModuleViewport, PreviewData?>(nameof(Data));
    public PreviewData? Data { get => GetValue(DataProperty); set => SetValue(DataProperty, value); }
    public static readonly StyledProperty<TextCatalog> TextProperty = AvaloniaProperty.Register<ModuleViewport, TextCatalog>(nameof(Text));
    public TextCatalog Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public static readonly StyledProperty<ModuleXmlReader.Presentation.ViewModels.SimulationViewModel?> SimulationProperty = AvaloniaProperty.Register<ModuleViewport, ModuleXmlReader.Presentation.ViewModels.SimulationViewModel?>(nameof(Simulation));
    public ModuleXmlReader.Presentation.ViewModels.SimulationViewModel? Simulation { get => GetValue(SimulationProperty); set => SetValue(SimulationProperty, value); }
    private ModuleXmlReader.Presentation.ViewModels.SimulationViewModel? _simulation;
    private readonly SimulationPanel _simulationPanel = new() { Width = 320, IsVisible = false };
    private TextCatalog? _subscribedText;
    private readonly TextBlock _help = new();
    private readonly Button _fitButton = new();
    public ModulePreview Preview { get; } = new();
    public ScrollViewer Scroller { get; }
    private readonly CanvasRuler _top = new(true), _left = new(false);
    private readonly TextBlock _status = new() { Foreground = Brush.Parse("#C8A0F5"), FontSize = 11 };
    private bool _fit = true, _panning;
    private Point _panStart;
    private Vector _panOffset;

    public ModuleViewport()
    {
        Focusable = true;
        Text = new TextCatalog();
        Scroller = new ScrollViewer
        {
            Content = Preview, AllowAutoHide = false, HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalContentAlignment = global::Avalonia.Layout.HorizontalAlignment.Left,
            VerticalContentAlignment = global::Avalonia.Layout.VerticalAlignment.Top
        };
        var root = new Grid { RowDefinitions = new("Auto,*"), RowSpacing = 6 };
        var toolbar = new DockPanel { LastChildFill = true };
        var fit = _fitButton; fit.Classes.Add("subtle"); fit.Margin = new Thickness(0, 0, 8, 0);
        fit.Click += (_, _) => { Focus(); FitToView(); };
        DockPanel.SetDock(fit, global::Avalonia.Controls.Dock.Right); toolbar.Children.Add(fit);
        var info = new StackPanel { Spacing = 2 };
        _help.FontSize = 12; _help.Foreground = Brush.Parse("#94A7C2");
        info.Children.Add(_help);
        info.Children.Add(_status); toolbar.Children.Add(info); root.Children.Add(toolbar);
        var grid = new Grid { RowDefinitions = new("28,*"), ColumnDefinitions = new("44,*"), ClipToBounds = true, Background = Brush.Parse("#142037") };
        var body = new Grid { ColumnDefinitions = new("*,Auto") };
        Grid.SetRow(body, 1); root.Children.Add(body); body.Children.Add(grid);
        Grid.SetColumn(_simulationPanel, 1); body.Children.Add(_simulationPanel);
        Grid.SetColumn(_top, 1); grid.Children.Add(_top);
        Grid.SetRow(_left, 1); grid.Children.Add(_left);
        Grid.SetRow(Scroller, 1); Grid.SetColumn(Scroller, 1); grid.Children.Add(Scroller);
        Content = root;
        Preview.Text = Text; RefreshLanguage();
        Scroller.ScrollChanged += (_, _) => UpdateRulers();
        Scroller.SizeChanged += (_, _) => { if (_fit) ScheduleFit(); };
        AddHandler(PointerWheelChangedEvent, Wheel, RoutingStrategies.Tunnel);
        AddHandler(PointerPressedEvent, Press, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, Move, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, Release, RoutingStrategies.Tunnel);
        PointerCaptureLost += (_, _) => StopPan();
        KeyDown += (_, e) => { if (e.Key == Key.Z && e.KeyModifiers == KeyModifiers.None) { FitToView(); e.Handled = true; } };
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SimulationProperty)
        {
            if (_simulation is not null) _simulation.PropertyChanged -= SimulationChanged;
            _simulation = Simulation;
            if (_simulation is not null) _simulation.PropertyChanged += SimulationChanged;
            _simulationPanel.DataContext = Simulation; _simulationPanel.IsVisible = Simulation is not null;
            UpdateSimulation(); return;
        }
        if (change.Property == TextProperty)
        {
            if (_subscribedText is not null) _subscribedText.LanguageChanged -= LanguageChanged;
            _subscribedText = Text;
            if (_subscribedText is not null) _subscribedText.LanguageChanged += LanguageChanged;
            Preview.Text = Text; RefreshLanguage();
            return;
        }
        if (change.Property != DataProperty) return;
        Preview.Data = Data;
        RefreshLanguage();
        if (_fit) ScheduleFit(); else { Scroller.Offset = default; UpdateRulers(); }
    }
    private void SimulationChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => UpdateSimulation();
    private void UpdateSimulation() { Preview.Frame = Simulation?.Frame; Preview.ShowGroups = Simulation?.ShowGroups == true; }
    private void LanguageChanged(object? sender, EventArgs e) => RefreshLanguage();
    private void RefreshLanguage()
    {
        if (Text is null) return;
        _fitButton.Content = Text["Fit"];
        _help.Text = Text["CanvasHelp"];
        _status.Text = Data?.UnlocatedMissing is { Count: > 0 } missing
            ? Text.Format("Unlocated", string.Join(", ", missing)) : Text["PurpleLegend"];
    }
    private void ScheduleFit() => global::Avalonia.Threading.Dispatcher.UIThread.Post(() => { if (_fit) FitToView(); },
        global::Avalonia.Threading.DispatcherPriority.Loaded);
    public void FitToView()
    {
        _fit = true;
        if (Data is not { } data) return;
        double width = Scroller.Viewport.Width > 0 ? Scroller.Viewport.Width : Scroller.Bounds.Width;
        double height = Scroller.Viewport.Height > 0 ? Scroller.Viewport.Height : Scroller.Bounds.Height;
        if (width <= 2 || height <= 2) return;
        // Reserve scrollbar thickness so visibility changes cannot cause a fit loop.
        Preview.CellSize = Math.Max(0.01, Math.Min(Math.Max(1, width - 18) / data.Cols, Math.Max(1, height - 18) / data.Rows));
        Scroller.Offset = default;
        UpdateRulers();
    }
    public void ZoomAt(double factor, Point viewportPoint)
    {
        if (Data is null || !double.IsFinite(factor) || factor <= 0) return;
        _fit = false;
        double old = Preview.CellSize, next = Math.Clamp(old * factor, 0.01, 96);
        var anchor = (Scroller.Offset + new Vector(viewportPoint.X, viewportPoint.Y)) / old;
        Preview.CellSize = next;
        UpdateLayout();
        Scroller.Offset = anchor * next - new Vector(viewportPoint.X, viewportPoint.Y);
        UpdateRulers();
    }
    private void Wheel(object? sender, PointerWheelEventArgs e)
    {
        if (Data is null || !new Rect(Scroller.Bounds.Size).Contains(e.GetPosition(Scroller))) return;
        Focus(); ZoomAt(Math.Pow(1.2, e.Delta.Y), e.GetPosition(Scroller)); e.Handled = true;
    }
    private void Press(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed || Data is null || !new Rect(Scroller.Bounds.Size).Contains(e.GetPosition(Scroller))) return;
        Focus(); _panning = true; _panStart = e.GetPosition(this); _panOffset = Scroller.Offset;
        Cursor = new Cursor(StandardCursorType.SizeAll); e.Pointer.Capture(this); e.Handled = true;
    }
    private void Move(object? sender, PointerEventArgs e)
    {
        if (!_panning) return;
        Scroller.Offset = _panOffset - (e.GetPosition(this) - _panStart); UpdateRulers(); e.Handled = true;
    }
    private void Release(object? sender, PointerReleasedEventArgs e)
    {
        if (!_panning || e.InitialPressMouseButton != MouseButton.Middle) return;
        e.Pointer.Capture(null); StopPan(); e.Handled = true;
    }
    private void StopPan() { _panning = false; Cursor = Cursor.Default; }
    private void UpdateRulers()
    {
        _top.Configure(Data?.Cols ?? 0, Preview.CellSize, Scroller.Offset.X);
        _left.Configure(Data?.Rows ?? 0, Preview.CellSize, Scroller.Offset.Y);
    }
}

internal sealed class CanvasRuler(bool horizontal) : Control
{
    private int _count;
    private double _cell = 18, _offset;
    public CanvasRuler() : this(true) { }
    public void Configure(int count, double cell, double offset)
    { _count = count; _cell = cell; _offset = offset; InvalidateVisual(); }
    public override void Render(DrawingContext context)
    {
        using var clip = context.PushClip(new Rect(Bounds.Size));
        context.FillRectangle(Brush.Parse("#142037"), new Rect(Bounds.Size));
        int stride = _cell >= 16 ? 1 : Math.Max(1, (int)Math.Ceiling((horizontal ? 25 : 16) / _cell));
        int first = Math.Max(0, (int)(_offset / _cell)); first -= first % stride;
        double length = horizontal ? Bounds.Width : Bounds.Height;
        int last = Math.Min(_count, (int)Math.Ceiling((_offset + length) / _cell) + 1);
        for (int i = first; i < last; i += stride)
        {
            var text = new FormattedText((i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, Typeface.Default,
                Math.Min(11, _cell * 0.55), Brush.Parse("#70B8FF"));
            double position = (i + 0.5) * _cell - _offset;
            context.DrawText(text, horizontal ? new Point(position - text.Width / 2, (Bounds.Height - text.Height) / 2)
                : new Point(Bounds.Width - text.Width - 5, position - text.Height / 2));
        }
    }
}

