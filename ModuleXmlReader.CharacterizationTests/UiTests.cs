using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Services;
using ModuleXmlReader.Presentation.ViewModels;
using Xunit;
[assembly: AvaloniaTestApplication(typeof(ModuleXmlReader.CharacterizationTests.TestApp))]
namespace ModuleXmlReader.CharacterizationTests;
public static class TestApp
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UseSkia()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
public sealed class UiTests
{
    [AvaloniaFact]
    public void MainWindowLoadsWithDisabledDocumentCommands()
    {
        var window = CompositionRoot.CreateMainWindow(); window.Show();
        Assert.Equal(28, window.FindControl<ItemsControl>("ParametersPanel")!.ItemCount);
        Assert.Equal("Загрузите файл модуля", window.FindControl<TextBlock>("MissingText")!.Text);
        foreach (var name in new[] { "SaveButton", "RotateButton" })
            Assert.False(window.FindControl<Button>(name)!.IsEffectivelyEnabled);
        window.Close();
    }
    [AvaloniaFact]
    public void RendererShowsWholeGridAndCoordinatesOutsideOldClippedArea()
    {
        var preview = new ModulePreview { CellSize = 10, Data = new(3, 2, new[] { new PreviewPoint(2, 1) }) };
        var window = new Window { Content = preview }; window.Show();
        Assert.Equal(30, preview.Width); Assert.Equal(20, preview.Height);
        using var bitmap = new RenderTargetBitmap(new PixelSize(30, 20)); bitmap.Render(preview);
        using var stream = new MemoryStream(); bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        using var pixels = SkiaSharp.SKBitmap.Decode(stream.ToArray());
        Assert.Equal(new SkiaSharp.SKColor(55, 196, 155), pixels.GetPixel(25, 15));
        Assert.Equal(new SkiaSharp.SKColor(38, 50, 71), pixels.GetPixel(5, 5)); window.Close();
    }
    [AvaloniaFact]
    public async Task BindingsUpdateValidationPreviewAndCyclicRotation()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture(), Choice = UnsavedChangesChoice.Discard };
        var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs);
        var window = new MainWindow { DataContext = vm }; window.Show();
        await vm.OpenCommand.ExecuteAsync(); Dispatcher.UIThread.RunJobs();
        var text = window.FindControl<TextBox>("PointTableText")!; var valid = text.Text;
        var preview = window.FindControl<ModuleViewport>("ModuleCanvas")!.Preview;
        Assert.Same(vm.Preview, preview.Data);
        text.Text = "1-2-3"; Dispatcher.UIThread.RunJobs();
        Assert.NotEmpty(vm.ValidationError); Assert.Null(preview.Data); Assert.False(window.FindControl<Button>("SaveButton")!.IsEffectivelyEnabled);
        text.Text = valid; Dispatcher.UIThread.RunJobs();
        var button = window.FindControl<Button>("RotateButton")!; Assert.Same(vm.RotateCommand, button.Command);
        Assert.Equal("Поворот: 0°", window.FindControl<TextBlock>("RotationLabelText")!.Text);
        button.Command!.Execute(null); Dispatcher.UIThread.RunJobs();
        Assert.Equal("Поворот: 90°", window.FindControl<TextBlock>("RotationLabelText")!.Text);
        button.Command!.Execute(null); Dispatcher.UIThread.RunJobs();
        Assert.Equal("Поворот: 180°", window.FindControl<TextBlock>("RotationLabelText")!.Text);
        Assert.StartsWith("127-31", text.Text); Assert.Same(editor.State.Preview, preview.Data);
        window.UpdateLayout();
        var buttonPosition = button.TranslatePoint(default, window);
        Assert.NotNull(buttonPosition);
        Assert.True(buttonPosition.Value.X + button.Bounds.Width <= window.ClientSize.Width);
        var editorPosition = text.TranslatePoint(default, window);
        Assert.NotNull(editorPosition);
        Assert.True(editorPosition.Value.X + text.Bounds.Width <= window.ClientSize.Width);
        var screenshot = Environment.GetEnvironmentVariable("MODULE_READER_SCREENSHOT");
        if (!string.IsNullOrEmpty(screenshot)) { using var frame = window.CaptureRenderedFrame(); Assert.NotNull(frame); frame.Save(screenshot, PngBitmapEncoderOptions.Default); }
        window.Close(); Dispatcher.UIThread.RunJobs();
    }
    [AvaloniaFact]
    public async Task WindowClosingHonorsUnsavedCancelAndDiscard()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture() };
        var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs);
        var window = new MainWindow { DataContext = vm }; window.Show(); await vm.OpenCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync();
        window.Close(); Dispatcher.UIThread.RunJobs(); Assert.True(window.IsVisible);
        dialogs.Choice = UnsavedChangesChoice.Discard; window.Close(); Dispatcher.UIThread.RunJobs(); Assert.False(window.IsVisible);
    }
    [AvaloniaFact]
    public void AboutWindowLoads() { var window = new AboutWindow(); window.Show(); Assert.NotNull(window); window.Close(); }
}
