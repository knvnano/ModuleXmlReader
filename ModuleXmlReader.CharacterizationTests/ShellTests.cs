using System.Text.Json;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Infrastructure;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Presentation.ViewModels;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class ShellTests : TempTest
{
    [Fact]
    public void EveryResourceHasAnEnglishTranslationAndStructuredErrorsTranslate()
    {
        var text = new TextCatalog(); var keys = text.Keys; text.SetLanguage("en");
        foreach (string key in keys)
        {
            Assert.True(text.HasTranslation(key), key); Assert.DoesNotMatch("[А-Яа-яЁё]", text[key]);
        }
        var error = Assert.Throws<FormatException>(() => new PointTableParser().Parse("1-2-3"));
        Assert.Contains("even number", text.Format(Messages.FromException(error)));
        Assert.Equal("plain {text}", text.Format(new MessageToken("Unknown", "plain {text}")));
    }
    [Fact]
    public void LanguageSwitchUpdatesValidationAndStateWithoutChangingModule()
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
        var text = new TextCatalog(); var dialogs = new FakeDialogs();
        using var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs, text);
        var points = vm.PointTableData; var code = vm.NewScanABCDCode;
        vm.PointTableData = "1-2-3"; Assert.Contains("чётное", vm.ValidationError);
        vm.ToggleLanguageCommand.Execute(null);
        Assert.Contains("even number", vm.ValidationError); Assert.Equal("1-2-3", vm.PointTableData);
        Assert.True(vm.HasChanges); Assert.Equal(code, vm.NewScanABCDCode); Assert.Null(vm.Preview);
        vm.PointTableData = points; Assert.Equal("No missing pixels found.", vm.MissingPixels);
        Assert.Equal("Rotation: 0°", vm.RotationLabel); Assert.False(vm.HasChanges);
    }
    [Fact]
    public async Task ChangelogReloadsSavedEntriesAndTranslatesImmediately()
    {
        string file = Path.Combine(Dir, "changelog.json");
        var entry = new ChangeEntry("1.0", "2026-11-23", "Изменение", "A change", new[] { "Пункт" }, new[] { "An item" });
        File.WriteAllText(file, JsonSerializer.Serialize(new[] { entry }));
        var dialogs = new FakeDialogs(); var text = new TextCatalog();
        using var vm = new MainWindowViewModel(TestServices.CreateEditor(), dialogs, dialogs, dialogs, dialogs, text, new JsonChangelogReader(file));
        await vm.ShowChangesCommand.ExecuteAsync(); Assert.True(vm.IsChangesPage); Assert.Equal("Изменение", Assert.Single(vm.ChangeEntries).Title);
        vm.ToggleLanguageCommand.Execute(null); Assert.Equal("A change", Assert.Single(vm.ChangeEntries).Title); Assert.Contains("An item", vm.ChangeEntries[0].Description);
        File.WriteAllText(file, JsonSerializer.Serialize(new[] { entry, entry with { Version = "1.1" } }));
        await vm.RefreshChangesCommand.ExecuteAsync(); Assert.Equal(2, vm.ChangeEntries.Count);
        vm.ToggleSidebarCommand.Execute(null); Assert.Equal(72, vm.SidebarWidth);
        vm.ShowEditorCommand.Execute(null); Assert.True(vm.IsEditorPage);
    }
    [Fact]
    public async Task LogsViewReadsEachSeparateFileAndRefreshes()
    {
        File.WriteAllText(Path.Combine(Dir, "activity.log"), "activity-entry");
        File.WriteAllText(Path.Combine(Dir, "errors.log"), "error-entry");
        File.WriteAllText(Path.Combine(Dir, "diagnostics.log"), "debug-entry");
        var dialogs = new FakeDialogs();
        using var vm = new MainWindowViewModel(TestServices.CreateEditor(), dialogs, dialogs, dialogs, dialogs, logs: new FileLogReader(Dir));
        await vm.ShowLogsCommand.ExecuteAsync(); Assert.True(vm.IsLogsPage); Assert.Equal("activity-entry", vm.LogText);
        await vm.ErrorLogsCommand.ExecuteAsync(); Assert.Equal("error-entry", vm.LogText);
        await vm.DiagnosticLogsCommand.ExecuteAsync(); Assert.Equal("debug-entry", vm.LogText);
        File.WriteAllText(Path.Combine(Dir, "diagnostics.log"), "updated");
        await vm.RefreshLogsCommand.ExecuteAsync(); Assert.Equal("updated", vm.LogText);
    }
    [Fact]
    public void Log4NetSeparatesLevelsPreservesExceptionsAndRotatesFiles()
    {
        using (var logger = new Log4NetLogger(Dir, "1KB", 2))
        {
            logger.Info("activity-only"); logger.Debug("diagnostic-only");
            try { throw new InvalidOperationException("test exception details"); }
            catch (Exception ex) { logger.Error("error-only", ex); }
        }
        string activity = File.ReadAllText(Path.Combine(Dir, "activity.log"));
        string errors = File.ReadAllText(Path.Combine(Dir, "errors.log"));
        string diagnostics = File.ReadAllText(Path.Combine(Dir, "diagnostics.log"));
        Assert.Contains("activity-only", activity); Assert.DoesNotContain("error-only", activity); Assert.DoesNotContain("diagnostic-only", activity);
        Assert.Contains("error-only", errors); Assert.Contains("InvalidOperationException", errors); Assert.Contains("test exception details", errors);
        Assert.Contains("diagnostic-only", diagnostics); Assert.DoesNotContain("activity-only", diagnostics);
        using (var logger = new Log4NetLogger(Dir, "1KB", 2))
            for (int i = 0; i < 40; i++) logger.Info("roll-" + i + new string('x', 200));
        Assert.InRange(Directory.GetFiles(Dir, "activity.log*").Length, 2, 3);
        Assert.Contains("roll-39", File.ReadAllText(Path.Combine(Dir, "activity.log")));
    }
    [Fact]
    public void LogReaderBoundsLargeFilesAndChangelogRejectsMalformedData()
    {
        File.WriteAllText(Path.Combine(Dir, "activity.log"), string.Concat(Enumerable.Repeat("line-data\n", 30000)) + "last-entry");
        string tail = new FileLogReader(Dir).ReadTail(LogKind.Activity);
        Assert.True(tail.Length <= 128 * 1024); Assert.EndsWith("last-entry", tail);
        var path = Path.Combine(Dir, "bad.json"); File.WriteAllText(path, "{bad");
        Assert.Throws<JsonException>(() => new JsonChangelogReader(path).Read());
    }
    [AvaloniaFact]
    public async Task DarkShellSwitchesLanguageAndPagesWithoutResettingCanvas()
    {
        var text = new TextCatalog(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture() };
        var history = new JsonChangelogReader(Path.Combine(AppContext.BaseDirectory, "changelog.json"));
        using var vm = new MainWindowViewModel(TestServices.CreateEditor(), dialogs, dialogs, dialogs, dialogs, text, history);
        var window = new MainWindow { DataContext = vm }; window.Show(); await vm.OpenCommand.ExecuteAsync();
        Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
        var canvas = window.FindControl<ModuleViewport>("ModuleCanvas")!;
        canvas.ZoomAt(4, new Point(20, 20)); window.UpdateLayout();
        var scale = canvas.Preview.CellSize; var preview = canvas.Preview.Data; var offset = canvas.Scroller.Offset;
        Assert.Equal("Открыть модуль", window.FindControl<Button>("OpenButton")!.Content);
        vm.ToggleLanguageCommand.Execute(null); Dispatcher.UIThread.RunJobs();
        Assert.Equal("Open module", window.FindControl<Button>("OpenButton")!.Content);
        Assert.Equal("Save", window.FindControl<Button>("SaveButton")!.Content);
        Assert.Same(preview, canvas.Preview.Data); Assert.Equal(scale, canvas.Preview.CellSize); Assert.Equal(offset, canvas.Scroller.Offset);
        await vm.ShowChangesCommand.ExecuteAsync(); Dispatcher.UIThread.RunJobs(); Assert.NotEmpty(vm.ChangeEntries); Assert.True(window.FindControl<Grid>("ChangesPage")!.IsVisible);
        vm.ShowEditorCommand.Execute(null); Dispatcher.UIThread.RunJobs(); Assert.True(window.FindControl<Grid>("EditorPage")!.IsVisible);
        var screenshot = Environment.GetEnvironmentVariable("MODULE_READER_SHELL_SCREENSHOT");
        if (!string.IsNullOrEmpty(screenshot))
        {
            canvas.Scroller.Offset = default; window.UpdateLayout();
            using (var frame = window.CaptureRenderedFrame()) { Assert.NotNull(frame); frame.Save(screenshot, PngBitmapEncoderOptions.Default); }
            text.SetLanguage("ru"); Dispatcher.UIThread.RunJobs();
            using (var frame = window.CaptureRenderedFrame()) { Assert.NotNull(frame); frame.Save(Path.ChangeExtension(screenshot, "ru.png"), PngBitmapEncoderOptions.Default); }
            await vm.ShowChangesCommand.ExecuteAsync(); Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            using (var frame = window.CaptureRenderedFrame()) { Assert.NotNull(frame); frame.Save(Path.ChangeExtension(screenshot, "changelog.png"), PngBitmapEncoderOptions.Default); }
        }
        window.Close();
    }
    [AvaloniaFact]
    public async Task OpenDialogsAndAboutWindowTranslateWithoutReopening()
    {
        var text = new TextCatalog(); var owner = new Window(); owner.Show();
        var service = new ModuleXmlReader.Avalonia.Services.AvaloniaDialogService(owner, new FakeLinks(), text);
        var pending = service.ShowAsync(new Notice("fallback", "Ошибка", new("PointsEmpty", "fallback")));
        var dialog = owner.OwnedWindows.OfType<Window>().Single();
        Assert.Equal("Ошибка", dialog.Title); text.Toggle(); Dispatcher.UIThread.RunJobs(); Assert.Equal("Error", dialog.Title);
        dialog.Close(); await pending;
        using var about = new AboutViewModel(new FakeLinks(), new FakeDialogs(), new FakeClose(), text);
        var window = new AboutWindow { DataContext = about }; window.Show(); Assert.Equal("About", window.Title);
        text.Toggle(); Dispatcher.UIThread.RunJobs(); Assert.Equal("О программе", window.Title);
        window.Close(); owner.Close();
    }
}
