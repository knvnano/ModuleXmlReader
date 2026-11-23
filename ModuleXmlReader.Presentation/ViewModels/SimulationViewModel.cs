using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Presentation.Services;
namespace ModuleXmlReader.Presentation.ViewModels;

public sealed class SimulationViewModel : ObservableObject, IDisposable
{
    public TextCatalog Text { get; }
    private readonly IAnimationClock _clock;
    private SimulationModel? _model;
    private ModuleSimulationEngine? _engine;
    private SimulationFrame? _frame;
    private bool _running, _enabled, _showGroups = true;
    private double _speed = 8;
    private int _outputHold;
    private IReadOnlyList<SimulationGroupViewModel> _groups = Array.Empty<SimulationGroupViewModel>();
    public SimulationFrame? Frame { get => _frame; private set => SetProperty(ref _frame, value); }
    public bool IsRunning { get => _running; private set => SetProperty(ref _running, value); }
    public bool IsEnabled { get => _enabled; private set => SetProperty(ref _enabled, value); }
    public bool ShowGroups { get => _showGroups; set => SetProperty(ref _showGroups, value); }
    public double Speed { get => _speed; set => SetProperty(ref _speed, Math.Clamp(double.IsFinite(value) ? value : 8, 1, 32)); }
    public bool CanSimulate => _engine is not null;
    public string PlayLabel => Text[IsRunning ? "SimPause" : "SimPlay"];
    public string PhaseLabel => Text[Frame?.Phase switch
    { SimulationPhase.ClockLow or SimulationPhase.ClockHigh => "SimShift", SimulationPhase.Latch => "SimLatch", SimulationPhase.Output => "SimOutput", _ => "SimReady" }];
    public string Status => _model?.Problem is { } error ? Text.Format(error) : Frame is { } f
        ? Text.Format("SimStatus", f.Address, f.DisplayAddress?.ToString() ?? "РІР‚вЂќ", f.Clock, f.TotalClocks) : Text["SimReady"];
    public string ClockSignal => "CLK " + (Frame?.ClockHigh == true ? "1" : "0");
    public string LatchSignal => "LAT " + (Frame?.LatchHigh == true ? "1" : "0");
    public string OeSignal => "OE " + (Frame?.OeLevel is bool high ? high ? "1" : "0" : "РІР‚вЂќ");
    public IReadOnlyList<SimulationGroupViewModel> Groups { get => _groups; private set => SetProperty(ref _groups, value); }
    public RelayCommand PlayPauseCommand { get; }
    public RelayCommand StepCommand { get; }
    public RelayCommand ResetCommand { get; }
    public SimulationViewModel(TextCatalog text, IAnimationClock clock)
    {
        Text = text; _clock = clock; clock.Tick += Tick; text.LanguageChanged += LanguageChanged;
        PlayPauseCommand = new(() => { if (_engine is null) return; IsEnabled = true; IsRunning = !IsRunning;
            if (IsRunning) _clock.Start(); else _clock.Stop(); Publish(); });
        StepCommand = new(() => { if (_engine is null) return; Pause(); IsEnabled = true; _engine.Step(); Publish(); });
        ResetCommand = new(() => { Pause(); _engine?.Reset(); IsEnabled = false; _outputHold = 0; Publish(); });
    }
    public void UpdateModel(SimulationModel? model)
    {
        if (ReferenceEquals(_model, model)) return;
        Pause(); _model = model; IsEnabled = false; _outputHold = 0;
        _engine = model is { Problem: null } && model.Points.Count > 0 && model.Addresses.Count > 0 ? new(model) : null;
        RaisePropertyChanged(nameof(CanSimulate)); Publish();
    }
    public void Pause() { IsRunning = false; _clock.Stop(); RaisePropertyChanged(nameof(PlayLabel)); }
    private void Tick(object? sender, EventArgs e)
    {
        if (!IsRunning || _engine is null) return;
        if (_engine.Phase is SimulationPhase.Output or SimulationPhase.Latch && _outputHold++ < (_engine.Phase == SimulationPhase.Latch ? 2 : 8)) return;
        _outputHold = 0;
        for (int i = 0; i < (int)Speed * 2 - 1; i++)
        {
            _engine.Step();
            if (_engine.Phase is SimulationPhase.Output or SimulationPhase.Latch) break;
        }
        Publish();
    }
    private void Publish()
    {
        Frame = IsEnabled ? _engine?.Frame() : null;
        var snapshot = _engine?.Frame();
        Groups = snapshot?.Groups.Select(g => new SimulationGroupViewModel(g, Text)).ToArray() ?? Array.Empty<SimulationGroupViewModel>();
        foreach (var name in new[] { nameof(PlayLabel), nameof(PhaseLabel), nameof(Status), nameof(ClockSignal), nameof(LatchSignal), nameof(OeSignal) }) RaisePropertyChanged(name);
    }
    private void LanguageChanged(object? sender, EventArgs e) => Publish();
    public void Dispose() { _clock.Tick -= Tick; Text.LanguageChanged -= LanguageChanged; _clock.Stop(); _clock.Dispose(); }
}
public sealed class SimulationGroupViewModel
{
    private static readonly string[] Colors = ["#65DEC8", "#F6B768", "#70A5FF", "#F287B7", "#B6A1FF", "#72D0F4", "#C9D46B", "#F58B6A"];
    public static string ColorFor(int input) => Colors[input % Colors.Length];
    public string Name { get; }
    public string Color { get; }
    public IReadOnlyList<SimulationRegisterViewModel> Cells { get; }
    public SimulationGroupViewModel(SimulationGroupFrame group, TextCatalog text)
    {
        Color = ColorFor(group.Group.InputId);
        Name = text.Format("SimGroup", group.Group.InputId + 1, group.Group.Slot + 1);
        Cells = group.Register.Select(p => new SimulationRegisterViewModel(p.Number == 0 ? "Р’В·" : p.Number.ToString(),
            p.Number == 0 ? "#18243A" : p.IsMissing ? "#9B5DE5" : Color)).ToArray();
    }
}
public sealed record SimulationRegisterViewModel(string Number, string Fill);
