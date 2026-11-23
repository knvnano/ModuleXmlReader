using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class CanvasTests
{
    [Theory]
    [InlineData(0)] [InlineData(90)] [InlineData(180)] [InlineData(270)]
    public void PointNumbersAndInferredMissingPositionsFollowRotation(int angle)
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture(angle)));
        var parser = new PointTableParser(); var points = parser.Parse(editor.State.PointTableData).ToArray();
        var expected = points[0]; foreach (int i in new[] { 0, 16, 32, 48 }) points[i] = new(255, 255);
        var raw = parser.Format(points); editor.UpdateTables(raw, editor.State.NewScanABCDCode, editor.State.ScanABCDCode);
        var pixels = editor.State.Preview!.Pixels!;
        Assert.Equal(new PreviewPixel(expected.X, expected.Y, 1, true, true), pixels[0]);
        Assert.Equal(new[] { 1, 17, 33, 49 }, pixels.Take(128).Where(p => p.IsMissing).Select(p => p.Number));
        Assert.Equal(Enumerable.Range(1, 128), pixels.Take(128).Select(p => p.Number));
        Assert.Empty(editor.State.Preview.UnlocatedMissing!); Assert.Equal(raw, editor.State.PointTableData);
    }
    [Fact]
    public void ArbitraryWiringDoesNotInventCoordinatesForMissingPoints()
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
        var parser = new PointTableParser(); var points = parser.Parse(editor.State.PointTableData).ToArray();
        points[0] = new(255, 255); points[1] = new(9, 9); points[2] = new(23, 7);
        editor.UpdateTables(parser.Format(points), editor.State.NewScanABCDCode, editor.State.ScanABCDCode);
        Assert.Equal(new[] { 1 }, editor.State.Preview!.UnlocatedMissing);
        Assert.DoesNotContain(editor.State.Preview.Pixels!, p => p.Number == 1);
    }
    [AvaloniaFact]
    public void MissingCellIsPurpleAndNumberLabelsAreRendered()
    {
        var preview = new ModulePreview { CellSize = 24, Data = new(3, 2, new[] { new PreviewPoint(1, 0) },
            Pixels: new[] { new PreviewPixel(0, 0, 1, true, true), new PreviewPixel(1, 0, 2) }) };
        preview.Measure(new Size(72, 48)); preview.Arrange(new Rect(0, 0, 72, 48));
        using var bitmap = new RenderTargetBitmap(new PixelSize(72, 48)); bitmap.Render(preview);
        using var stream = new MemoryStream(); bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        using var pixels = SkiaSharp.SKBitmap.Decode(stream.ToArray());
        Assert.Equal(new SkiaSharp.SKColor(155, 93, 229), pixels.GetPixel(2, 2));
        Assert.Contains(Enumerable.Range(4, 16).SelectMany(y => Enumerable.Range(28, 16).Select(x => pixels.GetPixel(x, y))),
            c => c.Red < 40 && c.Green < 40 && c.Blue < 40);
    }
    [AvaloniaFact]
    public void MouseWheelZoomShowsScrollbarsMiddleDragPansAndZFitsAgain()
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
        var canvas = new ModuleViewport { Data = editor.State.Preview };
        var window = new Window { Width = 700, Height = 420, Content = canvas }; window.Show();
        Dispatcher.UIThread.RunJobs(); window.UpdateLayout(); canvas.FitToView(); window.UpdateLayout();
        var initial = canvas.Preview.CellSize;
        Assert.True(canvas.Scroller.Extent.Width <= canvas.Scroller.Viewport.Width + 1);
        var point = canvas.Scroller.TranslatePoint(new Point(200, 140), window)!.Value;
        window.MouseWheel(point, new Vector(0, 8)); Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
        Assert.True(canvas.Preview.CellSize > initial);
        Assert.True(canvas.Scroller.Extent.Width > canvas.Scroller.Viewport.Width);
        Assert.True(canvas.Scroller.Extent.Height > canvas.Scroller.Viewport.Height);
        Assert.False(canvas.Scroller.AllowAutoHide);
        canvas.Scroller.Offset = new Vector(200, 200); window.UpdateLayout();
        var before = canvas.Scroller.Offset;
        window.MouseDown(point, MouseButton.Middle); window.MouseMove(point + new Vector(30, 25));
        window.MouseUp(point + new Vector(30, 25), MouseButton.Middle); Dispatcher.UIThread.RunJobs();
        Assert.True(canvas.Scroller.Offset.X < before.X); Assert.True(canvas.Scroller.Offset.Y < before.Y);
        window.KeyPress(Key.Z, RawInputModifiers.None, PhysicalKey.Z, "z"); window.KeyRelease(Key.Z, RawInputModifiers.None, PhysicalKey.Z, "z"); Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
        Assert.True(canvas.Scroller.Extent.Width <= canvas.Scroller.Viewport.Width + 1);
        Assert.True(canvas.Scroller.Extent.Height <= canvas.Scroller.Viewport.Height + 1);
        Assert.Equal(default, canvas.Scroller.Offset);
        window.Width = 900; window.Height = 600; window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
        Assert.True(canvas.Preview.CellSize > initial);
        var screenshot = Environment.GetEnvironmentVariable("MODULE_READER_CANVAS_SCREENSHOT");
        if (!string.IsNullOrEmpty(screenshot))
        {
            var parser = new PointTableParser(); var marked = parser.Parse(editor.State.PointTableData).ToArray();
            foreach (int i in new[] { 0, 16, 32, 48 }) marked[i] = new(255, 255);
            editor.UpdateTables(parser.Format(marked), editor.State.NewScanABCDCode, editor.State.ScanABCDCode);
            canvas.Data = editor.State.Preview;
            canvas.ZoomAt(3, new Point(0, 0)); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            using var frame = window.CaptureRenderedFrame(); Assert.NotNull(frame); frame.Save(screenshot, PngBitmapEncoderOptions.Default);
        }
        window.Close();
    }
}





