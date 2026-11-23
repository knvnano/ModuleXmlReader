using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
namespace ModuleXmlReader.Avalonia;
public partial class App : global::Avalonia.Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = CompositionRoot.CreateMainWindow(Program.Logger, Program.LogsDirectory is { } directory ? new ModuleXmlReader.Infrastructure.FileLogReader(directory) : null);
        base.OnFrameworkInitializationCompleted();
    }
}
