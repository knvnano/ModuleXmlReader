using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Domain;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class ScanLineTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 31)]
    [InlineData(90, 31, 0, 0, 0)]
    [InlineData(180, 127, 31, 127, 0)]
    [InlineData(270, 0, 127, 31, 127)]
    public void ScanLineFollowsNovaLctPositionAndAddressOrder(int angle, int x, int y, int lastX, int lastY)
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture(angle)));
        var line = editor.State.Preview!.ScanLine!;
        Assert.Equal(32, line.Count); // exclude the 96 padding entries
        Assert.Equal(new PreviewScanAddress(x, y, 0), line[0]);
        Assert.Equal(new PreviewScanAddress(lastX, lastY, 31), line[^1]);
        Assert.Equal(Enumerable.Range(0, 32), line.Select(p => p.Index));
    }
    [Fact]
    public void RotationUpdatesAddressLineAlongWithRunningPoints()
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
        editor.Rotate(Rotation.Right90);
        Assert.Equal(new PreviewScanAddress(31, 0, 0), editor.State.Preview!.ScanLine![0]);
        editor.Rotate(Rotation.Right90);
        Assert.Equal(new PreviewScanAddress(127, 31, 0), editor.State.Preview!.ScanLine![0]);
    }
    [AvaloniaFact]
    public void RendererDrawsRedAddressCellsWithBlackLabelsAndZoomsWithoutClipping()
    {
        var data = new PreviewData(3, 2, new[] { new PreviewPoint(0, 0), new PreviewPoint(2, 1) },
            new[] { new PreviewScanAddress(0, 0, 0), new PreviewScanAddress(0, 1, 16) });
        var preview = new ModulePreview { CellSize = 20, Data = data };
        var window = new Window { Content = preview }; window.Show();
        Assert.Equal(60, preview.Width); Assert.Equal(40, preview.Height);
        using var bitmap = new RenderTargetBitmap(new PixelSize(60, 40)); bitmap.Render(preview);
        using var stream = new MemoryStream(); bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        using var pixels = SkiaSharp.SKBitmap.Decode(stream.ToArray());
        Assert.Equal(new SkiaSharp.SKColor(255, 99, 118), pixels.GetPixel(2, 2));
        Assert.Equal(new SkiaSharp.SKColor(55, 196, 155), pixels.GetPixel(52, 32));
        Assert.Contains(Enumerable.Range(3, 14).SelectMany(y => Enumerable.Range(3, 14).Select(x => pixels.GetPixel(x, y))),
            color => color.Red < 40 && color.Green < 40 && color.Blue < 40);
        preview.CellSize = 12; Assert.Equal(36, preview.Width); Assert.Equal(24, preview.Height);
        window.Close();
        var screenshot = Environment.GetEnvironmentVariable("MODULE_READER_SCAN_PREVIEW");
        if (!string.IsNullOrEmpty(screenshot))
        {
            var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
            var realPreview = new ModulePreview { Data = editor.State.Preview };
            var size = new Size(realPreview.Width, realPreview.Height);
            realPreview.Measure(size); realPreview.Arrange(new Rect(size));
            using var full = new RenderTargetBitmap(new PixelSize((int)size.Width, (int)size.Height));
            full.Render(realPreview); full.Save(screenshot, PngBitmapEncoderOptions.Default);
        }
    }
}
