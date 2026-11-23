using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

public sealed class ModuleSimulationService : IModuleSimulationService
{
    public SimulationModel Create(ValidatedModule module, ModuleEditorState state)
    {
        var g = module.Geometry; var groups = new List<SimulationGroup>();
        var addresses = module.ScanAddresses ?? Array.Empty<int>();
        bool? oeLow = state.Parameters["OEPolarity"] switch { "LowEnable" => true, "HighEnable" => false, _ => null };
        MessageToken? problem = null;
        if (state.Parameters["ScreenDriveType"] != "Concurrent") problem = new("SimUnsupportedDrive", "Unsupported drive mode");
        else if (addresses.Count == 0) problem = new("SimNoAddresses", "No scan addresses");
        else if (state.Parameters["LineBias"] is not ("" or "0")) problem = new("SimLineBias", "Nonzero LineBias is not supported by this logical model");
        int[] sequence;
        try { sequence = new ScanCodeService().Parse(state.Parameters["DataGroupSequence"], "DataGroupSequence"); }
        catch (FormatException) { sequence = []; }
        if (g.DataGroups == 1 && sequence.Length == 0) sequence = [0];
        if (sequence.Length < g.DataGroups || !sequence.Take(g.DataGroups).Order().SequenceEqual(Enumerable.Range(0, g.DataGroups)))
            problem ??= new("SimGroupOrder", "Unknown data group order");
        else
            for (int i = 0; i < g.DataGroups; i++) groups.Add(new(sequence[i], i,
                g.IsVertical ? i * g.GroupWidth : 0, g.IsVertical ? 0 : i * g.GroupHeight, g.GroupWidth, g.GroupHeight));
        if (problem is null)
            foreach (int address in addresses)
                foreach (var p in module.Points.Where(p => !p.IsMissing))
                {
                    int delta = address - addresses[0];
                    int x = p.X + (g.IsVertical ? delta : 0), y = p.Y + (g.IsVertical ? 0 : delta);
                    if (x < 0 || x >= g.GroupWidth || y < 0 || y >= g.GroupHeight)
                    { problem = new("SimUnsupportedMapping", "Scan offsets leave the group bounds"); break; }
                }
        return new(g, module.Points, addresses, groups.AsReadOnly(), oeLow, problem);
    }
}
