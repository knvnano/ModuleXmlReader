namespace ModuleXmlReader.Presentation.Services;

// Visual persistence only; it never changes logical scan or register state.
public sealed class LightEnvelope
{
    public double Value { get; private set; }
    public double Target { get; set; }
    public bool IsSettled => Math.Abs(Target - Value) < 0.002;
    public void Advance(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds <= 0) return;
        double tau = Target > Value ? 0.065 : 0.14;
        Value = Target + (Value - Target) * Math.Exp(-seconds / tau);
        if (IsSettled) Value = Target;
    }
}
