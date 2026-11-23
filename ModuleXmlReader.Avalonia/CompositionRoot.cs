using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Infrastructure;
using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Avalonia.Services;
namespace ModuleXmlReader.Avalonia;

// Constructor injection, assembled explicitly in one composition root.
// No service locator or UI/persistence dependency in the inner layers.
public static class CompositionRoot
{
    public static MainWindow CreateMainWindow(IAppLogger? logger = null, ILogReader? logs = null)
    {
        var parser = new PointTableParser();
        var scan = new ScanCodeService();
        var validation = new ModuleValidationService(parser, scan);
        var editor = new ModuleEditorService(new XmlModuleRepository(new XmlModuleSerializer()),
            new MissingPixelService(parser), new ModuleRotationService(parser, scan, validation),
            validation, new PreviewService(), logger, new ModuleSimulationService());
        var window = new MainWindow();
        var text = new TextCatalog();
        var dialogs = new AvaloniaDialogService(window, new ExternalLinkService(), text, logger);
        window.DataContext = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs, text,
            new JsonChangelogReader(Path.Combine(AppContext.BaseDirectory, "changelog.json")), logs, logger, new DispatcherAnimationClock());
        window.Closed += (_, _) => ((MainWindowViewModel)window.DataContext!).Dispose();
        return window;
    }
}
