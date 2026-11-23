namespace ModuleXmlReader.Presentation.Services;
public interface IAnimationClock : IDisposable
{
    event EventHandler? Tick;
    void Start();
    void Stop();
}
public sealed class ManualAnimationClock : IAnimationClock
{
    public event EventHandler? Tick;
    private bool _running;
    public void Start() => _running = true;
    public void Stop() => _running = false;
    public void Advance() { if (_running) Tick?.Invoke(this, EventArgs.Empty); }
    public void Dispose() => Stop();
}
