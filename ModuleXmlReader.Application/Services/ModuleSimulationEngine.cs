using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

// Educational scan pipeline, not a cycle-accurate IC emulator. Each table entry
// occupies a shift clock, including a missing 255-255 entry. Groups shift in parallel.
public sealed class ModuleSimulationEngine
{
    public SimulationModel Model { get; }
    private int _scan, _clock, _loaded, _display = -1;
    private SimulationPhase _phase;
    public SimulationPhase Phase => _phase;
    public ModuleSimulationEngine(SimulationModel model)
    {
        if (model.Problem is not null || model.Addresses.Count == 0 || model.Points.Count == 0)
            throw new ArgumentException("The simulation model is unavailable.", nameof(model));
        Model = model;
    }
    public void Reset() { _scan = _clock = _loaded = 0; _display = -1; _phase = SimulationPhase.ClockLow; }
    public void Step()
    {
        switch (_phase)
        {
            case SimulationPhase.ClockLow: _loaded++; _phase = SimulationPhase.ClockHigh; break;
            case SimulationPhase.ClockHigh:
                if (_loaded == Model.Points.Count) _phase = SimulationPhase.Latch;
                else { _clock++; _phase = SimulationPhase.ClockLow; }
                break;
            case SimulationPhase.Latch: _display = _scan; _phase = SimulationPhase.Output; break;
            case SimulationPhase.Output:
                _scan = (_scan + 1) % Model.Addresses.Count; _clock = _loaded = 0; _phase = SimulationPhase.ClockLow; break;
        }
    }
    public SimulationFrame Frame()
    {
        bool enabled = _display >= 0 && _phase != SimulationPhase.Latch;
        bool? oe = Model.OeActiveLow is bool low ? (enabled ? !low : low) : null;
        var register = Enumerable.Range(0, 8).Select(i => i < _loaded
            ? new SimulationRegisterCell(_loaded - i, Model.Points[_loaded - i - 1].IsMissing)
            : new SimulationRegisterCell(0, false)).ToArray();
        var groups = Model.Groups.Select(group =>
        {
            var lit = new List<PreviewPoint>();
            if (enabled)
                foreach (var p in Model.Points.Where(p => !p.IsMissing)) lit.Add(Map(p, _display, group));
            PreviewPoint? loading = _loaded > 0 && !Model.Points[_loaded - 1].IsMissing
                ? Map(Model.Points[_loaded - 1], _scan, group) : null;
            return new SimulationGroupFrame(group, lit.AsReadOnly(), loading, Array.AsReadOnly(register));
        }).ToArray();
        return new(_scan, Model.Addresses[_scan], _display >= 0 ? Model.Addresses[_display] : null,
            _clock + 1, Model.Points.Count, _phase, _phase == SimulationPhase.ClockHigh,
            _phase == SimulationPhase.Latch, enabled, oe, groups);
    }
    private PreviewPoint Map(PointTableEntry p, int scanIndex, SimulationGroup group)
    {
        int delta = Model.Addresses[scanIndex] - Model.Addresses[0];
        return new(group.X + p.X + (Model.Geometry.IsVertical ? delta : 0),
            group.Y + p.Y + (Model.Geometry.IsVertical ? 0 : delta));
    }
}
