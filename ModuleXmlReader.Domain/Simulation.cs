namespace ModuleXmlReader.Domain;

public enum SimulationPhase { ClockLow, ClockHigh, Latch, Output }
public sealed record SimulationGroup(int InputId, int Slot, int X, int Y, int Width, int Height);
public sealed record SimulationModel(ModuleGeometry Geometry, IReadOnlyList<PointTableEntry> Points,
    IReadOnlyList<int> Addresses, IReadOnlyList<SimulationGroup> Groups, bool? OeActiveLow, MessageToken? Problem = null);
public sealed record SimulationRegisterCell(int Number, bool IsMissing);
public sealed record SimulationGroupFrame(SimulationGroup Group, IReadOnlyList<PreviewPoint> LitPoints,
    PreviewPoint? LoadingPoint, IReadOnlyList<SimulationRegisterCell> Register);
public sealed record SimulationFrame(int AddressIndex, int Address, int? DisplayAddress, int Clock,
    int TotalClocks, SimulationPhase Phase, bool ClockHigh, bool LatchHigh, bool OutputsEnabled,
    bool? OeLevel, IReadOnlyList<SimulationGroupFrame> Groups);
