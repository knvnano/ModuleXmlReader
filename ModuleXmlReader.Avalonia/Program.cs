using Avalonia;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Infrastructure;
namespace ModuleXmlReader.Avalonia;
internal static class Program
{
    internal static IAppLogger Logger { get; private set; } = NullAppLogger.Instance;
    internal static string? LogsDirectory { get; private set; }
    [STAThread]
    public static void Main(string[] args)
    {
        LogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ModuleXmlReader", "Logs", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Environment.ProcessId);
        using var logger = new Log4NetLogger(LogsDirectory);
        Logger = logger;
        logger.Info("Application starting; version="+typeof(Program).Assembly.GetName().Version);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => logger.Error("Unhandled process exception",
            e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString()));
        TaskScheduler.UnobservedTaskException += (_, e) => logger.Error("Unobserved task exception", e.Exception);
        global::Avalonia.Threading.Dispatcher.UIThread.UnhandledException += (_, e) => logger.Error("Unhandled UI exception", e.Exception);
        try { BuildAvaloniaApp().StartWithClassicDesktopLifetime(args); }
        catch (Exception ex) { logger.Error("Application terminated unexpectedly", ex); throw; }
        finally { logger.Info("Application stopped"); Logger = NullAppLogger.Instance; }
    }
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect();
}
