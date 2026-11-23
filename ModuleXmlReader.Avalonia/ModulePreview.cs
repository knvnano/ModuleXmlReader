using ModuleXmlReader.Presentation.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Avalonia;

public sealed class ModulePreview : Control
{
    private static readonly IBrush GridBrush = Brush.Parse("#263247"), TraceBrush = Brush.Parse("#37C49B"),
        MissingBrush = Brush.Parse("#9B5DE5"), AddressBrush = Brush.Parse("#FF6376");
    private static readonly Pen GridPen = new(Brush.Parse("#3B4960"), 0.5);
    private readonly Dictionary<(int Input, int X, int Y, bool Head), ModuleXmlReader.Presentation.Services.LightEnvelope> _lights = new();
    private readonly global::Avalonia.Threading.DispatcherTimer _fadeTimer = new() { Interval = TimeSpan.FromMilliseconds(16) };
    private bool _attached;
    private readonly System.Diagnostics.Stopwatch _fadeWatch = new();
    private void SetLightTargets()
    {
        if (_fadeWatch.IsRunning) AdvanceLights();
        foreach (var light in _lights.Values) light.Target = 0;
        if (Frame is { } frame)
            foreach (var group in frame.Groups)
            {
                foreach (var point in group.LitPoints) Target(group.Group.InputId, point, false);
                if (group.LoadingPoint is { } head) Target(group.Group.InputId, head, true);
            }
        if (_lights.Count > 0 && _attached) { _fadeWatch.Restart(); _fadeTimer.Start(); }
    }
    private void Target(int input, PreviewPoint point, bool head)
    {
        var key = (input, point.X, point.Y, head);
        if (!_lights.TryGetValue(key, out var light)) _lights[key] = light = new();
        light.Target = 1;
    }
    private void AdvanceLights()
    {
        double elapsed = _fadeWatch.Elapsed.TotalSeconds; _fadeWatch.Restart();
        foreach (var light in _lights.Values) light.Advance(elapsed);
        foreach (var key in _lights.Where(p => p.Value.Target == 0 && p.Value.Value == 0).Select(p => p.Key).ToArray()) _lights.Remove(key);
        InvalidateVisual();
        if (_lights.Values.All(light => light.IsSettled)) { _fadeTimer.Stop(); _fadeWatch.Stop(); }
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    { base.OnAttachedToVisualTree(e); _attached = true; SetLightTargets(); }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    { _attached = false; _fadeTimer.Stop(); _fadeWatch.Stop(); _lights.Clear(); base.OnDetachedFromVisualTree(e); }
    public int SelectedGroup { get; set; } = -1;
    public bool TraceMode { get; set; }
    public int TraceNumber { get; set; }
    public event Action<int>? PixelClicked;
    protected override void OnPointerPressed(global::Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e); if(!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)return;
        var pos=e.GetPosition(this);
        if(Data?.Simulation?.Groups.FirstOrDefault(g=>g.InputId==SelectedGroup) is { } selected && !new Rect(selected.X*CellSize,selected.Y*CellSize,selected.Width*CellSize,selected.Height*CellSize).Contains(pos)) return;
        int x=(int)(pos.X/CellSize),y=(int)(pos.Y/CellSize);
        if(Data?.Pixels?.FirstOrDefault(p=>p.X==x&&p.Y==y) is { Number:>0 } point) PixelClicked?.Invoke(point.Number);
    }
    public IReadOnlyList<PreviewPoint> Selection { get; set; } = Array.Empty<PreviewPoint>();
    public TextCatalog Text { get; set; } = new();
    public static readonly StyledProperty<PreviewData?> DataProperty = AvaloniaProperty.Register<ModulePreview, PreviewData?>(nameof(Data));
    public PreviewData? Data { get => GetValue(DataProperty); set => SetValue(DataProperty, value); }
    public static readonly StyledProperty<double> CellSizeProperty = AvaloniaProperty.Register<ModulePreview, double>(nameof(CellSize), 18,
        validate: value => double.IsFinite(value) && value > 0);
    public double CellSize { get => GetValue(CellSizeProperty); set => SetValue(CellSizeProperty, value); }
    public static readonly StyledProperty<SimulationFrame?> FrameProperty = AvaloniaProperty.Register<ModulePreview, SimulationFrame?>(nameof(Frame));
    public SimulationFrame? Frame { get => GetValue(FrameProperty); set => SetValue(FrameProperty, value); }
    public static readonly StyledProperty<bool> ShowGroupsProperty = AvaloniaProperty.Register<ModulePreview, bool>(nameof(ShowGroups));
    public bool ShowGroups { get => GetValue(ShowGroupsProperty); set => SetValue(ShowGroupsProperty, value); }
    static ModulePreview() => AffectsRender<ModulePreview>(DataProperty, CellSizeProperty, FrameProperty, ShowGroupsProperty);
    public ModulePreview() { Width = Height = 0; ClipToBounds = true; _fadeTimer.Tick += (_, _) => AdvanceLights(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DataProperty || change.Property == FrameProperty && Frame is null)
        { _lights.Clear(); _fadeTimer.Stop(); _fadeWatch.Stop(); }
        if (change.Property == FrameProperty) SetLightTargets();
        if (change.Property == DataProperty || change.Property == CellSizeProperty)
        { Width = (Data?.Cols ?? 0) * CellSize; Height = (Data?.Rows ?? 0) * CellSize; }
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Data is not { } data) return;
        double cell = CellSize;
        context.FillRectangle(GridBrush, new Rect(0, 0, Width, Height));
        var selected=data.Simulation?.Groups.FirstOrDefault(g=>g.InputId==SelectedGroup);
        using var groupClip=context.PushClip(selected is null ? new Rect(0,0,Width,Height) : new Rect(selected.X*cell,selected.Y*cell,selected.Width*cell,selected.Height*cell));
        foreach (var p in data.Points) context.FillRectangle(Frame is null ? GroupBrush(data,p.X,p.Y) : Brush.Parse("#304D58"), new Rect(p.X * cell, p.Y * cell, cell, cell));
        var missing = data.Pixels?.Where(p => p.IsMissing).Select(p => (p.X, p.Y)).ToHashSet() ?? [];
        if (data.Pixels is { } pixels)
            foreach (var p in pixels.Where(p => p.IsMissing))
                context.FillRectangle(MissingBrush, new Rect(p.X * cell, p.Y * cell, cell, cell));
        if (data.ScanLine is { } scanLine)
            foreach (var address in scanLine)
                if (!missing.Contains((address.X, address.Y)))
                    context.FillRectangle(AddressBrush, new Rect(address.X * cell, address.Y * cell, cell, cell));
        if (cell >= 4)
        {
            var pen = GridPen;
            for (int x = 0; x <= data.Cols; x++) context.DrawLine(pen, new Point(x * cell, 0), new Point(x * cell, Height));
            for (int y = 0; y <= data.Rows; y++) context.DrawLine(pen, new Point(0, y * cell), new Point(Width, y * cell));
        }
        DrawSimulation(context, data); DrawTrace(context,data);
        if (cell >= 10 && data.Pixels is { } numbered)
        {
            var addresses = data.ScanLine?.Select(p => (p.X, p.Y)).ToHashSet() ?? [];
            foreach (var pixel in numbered)
                if (pixel.IsMissing || !addresses.Contains((pixel.X, pixel.Y)))
                    DrawLabel(context, pixel.Number.ToString(System.Globalization.CultureInfo.InvariantCulture), pixel.X, pixel.Y,
                        pixel.IsMissing || Frame is not null ? Brushes.White : Brushes.Black);
        }
        if (cell >= 10 && data.ScanLine is { } labeledLine)
            foreach (var address in labeledLine)
            {
                if (missing.Contains((address.X, address.Y))) continue;
                string label = address.Index < 16 ? ((char)('A' + address.Index)).ToString()
                    : address.Index.ToString(System.Globalization.CultureInfo.InvariantCulture);
                DrawLabel(context, label, address.X, address.Y, Brushes.Black);
            }
        foreach(var p in Selection)
            context.DrawRectangle(null,new Pen(Brushes.Yellow,Math.Max(2,cell*0.1)),new Rect(p.X*cell,p.Y*cell,cell,cell).Deflate(1));
        // These mark a break in table order at known neighbouring cells, not
        // a guessed physical coordinate of a 255-255 entry. Paint last so a
        // grid line, number or red address cell cannot hide the separator.
        if (data.GapMarkers is { } gaps)
            foreach (var gap in gaps) context.FillRectangle(MissingBrush, GapRectangle(gap));
    }
    private IBrush GroupBrush(PreviewData data,int x,int y)
    {
        if(ShowGroups && data.Simulation?.Groups.FirstOrDefault(g=>x>=g.X&&x<g.X+g.Width&&y>=g.Y&&y<g.Y+g.Height) is { } group)
            return Brush.Parse(ModuleXmlReader.Presentation.ViewModels.SimulationGroupViewModel.ColorFor(group.InputId));
        return TraceBrush;
    }
    private void DrawTrace(DrawingContext context,PreviewData data)
    {
        if(!TraceMode || data.Simulation is not { } model)return;
        double c=CellSize;
        foreach(var group in model.Groups)
        {
            var brush=Brush.Parse(ModuleXmlReader.Presentation.ViewModels.SimulationGroupViewModel.ColorFor(group.InputId));
            for(int i=1;i<model.Points.Count;i++)
            {
                var a=model.Points[i-1];var b=model.Points[i]; if(a.IsMissing||b.IsMissing||c<8)continue;
                var start=new Point((group.X+a.X+0.5)*c,(group.Y+a.Y+0.5)*c);var end=new Point((group.X+b.X+0.5)*c,(group.Y+b.Y+0.5)*c);
                var delta=new Vector(end.X-start.X,end.Y-start.Y);if(delta.Length<0.1)continue;var unit=delta/delta.Length;var tip=start+delta*0.72;
                var pen=new Pen(brush,Math.Max(1,c*0.04));context.DrawLine(pen,start,end);
                context.DrawLine(pen,tip,tip-unit*(c*0.22)+new Vector(-unit.Y,unit.X)*(c*0.13));
                context.DrawLine(pen,tip,tip-unit*(c*0.22)-new Vector(-unit.Y,unit.X)*(c*0.13));
            }
            if(TraceNumber>0 && TraceNumber<=model.Points.Count && !model.Points[TraceNumber-1].IsMissing)
            {
                var p=model.Points[TraceNumber-1];var rect=new Rect((group.X+p.X)*c,(group.Y+p.Y)*c,c,c);
                context.FillRectangle(new SolidColorBrush(Colors.White,0.25),rect);context.DrawRectangle(null,new Pen(Brushes.Yellow,2),rect.Deflate(1));
            }
        }
    }
    private void DrawSimulation(DrawingContext context, PreviewData data)
    {
        double c = CellSize;
        if (ShowGroups && data.Simulation is { } model)
            foreach (var g in model.Groups)
            {
                var color = Color.Parse(ModuleXmlReader.Presentation.ViewModels.SimulationGroupViewModel.ColorFor(g.InputId));
                var area = new Rect(g.X * c, g.Y * c, g.Width * c, g.Height * c);
                context.FillRectangle(new SolidColorBrush(color, 0.055), area);
                context.DrawRectangle(null, new Pen(new SolidColorBrush(color, 0.7), Math.Clamp(c * 0.08, 1, 3)), area.Deflate(1));
            }
        foreach (var (key, light) in _lights)
        {
            double brightness = light.Value;
            if (brightness <= 0) continue;
            var color = Color.Parse(ModuleXmlReader.Presentation.ViewModels.SimulationGroupViewModel.ColorFor(key.Input));
            var center = new Point((key.X + 0.5) * c, (key.Y + 0.5) * c);
            if (key.Head)
            {
                context.DrawEllipse(new SolidColorBrush(color, brightness * 0.12), null, center, c * 0.49, c * 0.49);
                context.DrawEllipse(null, new Pen(new SolidColorBrush(color, brightness * 0.9), Math.Max(1, c * 0.08)), center, c * 0.40, c * 0.40);
                continue;
            }
            context.DrawEllipse(new SolidColorBrush(color, brightness * 0.15), null, center, c * 0.49, c * 0.49);
            context.DrawEllipse(new SolidColorBrush(color, brightness * 0.35), null, center, c * 0.37, c * 0.37);
            context.DrawEllipse(new SolidColorBrush(color, brightness), null, center, c * 0.24, c * 0.24);
            context.DrawEllipse(new SolidColorBrush(Colors.White, brightness), null, center, c * 0.09, c * 0.09);
        }
    }

    private Rect GapRectangle(PreviewGapMarker gap)
    {
        double cell = CellSize, width = Math.Min(cell, Math.Clamp(cell * 0.12, 1.5, 5));
        double x = gap.X * cell, y = gap.Y * cell;
        return gap.Edge switch
        {
            GapEdge.Left => new Rect(x, y, width, cell),
            GapEdge.Right => new Rect(x + cell - width, y, width, cell),
            GapEdge.Top => new Rect(x, y, cell, width),
            _ => new Rect(x, y + cell - width, cell, width)
        };
    }
    private void DrawLabel(DrawingContext context, string label, int x, int y, IBrush brush)
    {
        double size = Math.Min(CellSize * 0.78, CellSize * 1.3 / label.Length);
        var text = new FormattedText(label, System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Bold), size, brush);
        context.DrawText(text, new Point(x * CellSize + (CellSize - text.Width) / 2, y * CellSize + (CellSize - text.Height) / 2));
    }
    protected override void OnPointerMoved(global::Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var position = e.GetPosition(this);
        if (Data?.GapMarkers is { } gaps)
            foreach (var gap in gaps)
                if (GapRectangle(gap).Contains(position))
                {
                    string range = gap.Count == 1 ? gap.FirstMissing.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        : $"{gap.FirstMissing}РІР‚вЂњ{gap.FirstMissing + gap.Count - 1}";
                    ToolTip.SetTip(this, Text.Format("GapTip", range));
                    return;
                }
        int x = (int)(position.X / CellSize), y = (int)(position.Y / CellSize);
        var point = Data?.Pixels?.FirstOrDefault(p => p.X == x && p.Y == y);
        ToolTip.SetTip(this, point is { Number: > 0 } p ? Text.Format(
            !p.IsMissing ? "PixelTip" : p.IsInferred ? "InferredTip" : "MissingTip", p.Number) : null);
    }
}
