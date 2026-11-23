using Avalonia.Threading;
using ModuleXmlReader.Presentation.Services;
namespace ModuleXmlReader.Avalonia.Services;
public sealed class DispatcherAnimationClock : IAnimationClock
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(33) };
    public event EventHandler? Tick;
    public DispatcherAnimationClock() => _timer.Tick += OnTick;
    private void OnTick(object? sender, EventArgs e) => Tick?.Invoke(this, EventArgs.Empty);
    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void Dispose() { _timer.Stop(); _timer.Tick -= OnTick; }
}
