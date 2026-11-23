using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class GapMarkerTests
{
    private static PointTableEntry[] RepeatedTrace() => new[] { 14, 0 }.SelectMany(y =>
        Enumerable.Range(0, 6).SelectMany(chunk => new[] { new PointTableEntry(255, 255), new PointTableEntry(255, 255) }
            .Concat(Enumerable.Range(chunk * 14, 14).Select(x => new PointTableEntry(x, y))))).ToArray();

    [Fact]
    public void UnlocatedPairGapsHighlightExistingNeighboursWithoutCreatingCoordinates()
    {
        var input = RepeatedTrace();
        var data = new PreviewService().Create(new(new(84, 28, 1, false), input, Enumerable.Range(0, 14).ToArray()));
        Assert.Contains(17, data.UnlocatedMissing!); Assert.DoesNotContain(data.Pixels!, p => p.Number is 17 or 18);
        Assert.Contains(new PreviewGapMarker(13, 14, GapEdge.Right, 17, 2), data.GapMarkers!);
        Assert.Contains(new PreviewGapMarker(14, 14, GapEdge.Left, 17, 2), data.GapMarkers!);
        Assert.Contains(new PreviewGapMarker(0, 14, GapEdge.Left, 1, 2), data.GapMarkers!);
        // A row transition marks both endpoints rather than connecting distant cells.
        Assert.Contains(new PreviewGapMarker(83, 14, GapEdge.Right, 97, 2), data.GapMarkers!);
        Assert.Contains(new PreviewGapMarker(0, 0, GapEdge.Left, 97, 2), data.GapMarkers!);
        Assert.Equal(new PointTableEntry(255, 255), input[16]);
    }
    [Theory]
    [InlineData(0, GapEdge.Right, GapEdge.Left)]
    [InlineData(90, GapEdge.Bottom, GapEdge.Top)]
    [InlineData(180, GapEdge.Left, GapEdge.Right)]
    [InlineData(270, GapEdge.Top, GapEdge.Bottom)]
    public void GapBoundariesFollowRotatedTrace(int angle, GapEdge previous, GapEdge next)
    {
        var points = RepeatedTrace().Select(p => p.IsMissing ? p : angle switch
        {
            90 => new PointTableEntry(27 - p.Y, p.X), 180 => new PointTableEntry(83 - p.X, 27 - p.Y),
            270 => new PointTableEntry(p.Y, 83 - p.X), _ => p
        }).ToArray();
        bool vertical = angle is 90 or 270;
        var data = new PreviewService().Create(new(new(vertical ? 28 : 84, vertical ? 84 : 28, 1, vertical), points));
        var markers = data.GapMarkers!.Where(m => m.FirstMissing == 17).ToArray();
        Assert.Equal(2, markers.Length); Assert.Equal(previous, markers[0].Edge); Assert.Equal(next, markers[1].Edge);
    }
    [Fact]
    public void TrailingGapsAllMissingAndRepeatedGroupsAreHandled()
    {
        var input = new[] { new PointTableEntry(0, 0), new PointTableEntry(1, 0), new PointTableEntry(255, 255) };
        var data = new PreviewService().Create(new(new(3, 4, 2, false), input));
        Assert.Contains(new PreviewGapMarker(1, 0, GapEdge.Right, 3, 1), data.GapMarkers!);
        Assert.Contains(new PreviewGapMarker(1, 2, GapEdge.Right, 3, 1), data.GapMarkers!);
        var empty = new PreviewService().Create(new(new(3, 2, 1, false), new[] { new PointTableEntry(255, 255) }));
        Assert.Empty(empty.GapMarkers!); Assert.Equal(new[] { 1 }, empty.UnlocatedMissing);
    }
    [AvaloniaFact]
    public void PurpleSeparatorRemainsVisibleOverRedAddressCellsAndBetweenNumberedNeighbours()
    {
        var data = new PreviewData(2, 1, new[] { new PreviewPoint(0, 0), new PreviewPoint(1, 0) },
            new[] { new PreviewScanAddress(0, 0, 0) }, new[] { new PreviewPixel(0, 0, 3), new PreviewPixel(1, 0, 4) },
            GapMarkers: new[] { new PreviewGapMarker(0, 0, GapEdge.Left, 1, 2), new PreviewGapMarker(1, 0, GapEdge.Right, 5, 1) });
        var preview = new ModulePreview { Data = data, CellSize = 24 };
        preview.Measure(new Size(48, 24)); preview.Arrange(new Rect(0, 0, 48, 24));
        using var bitmap = new RenderTargetBitmap(new PixelSize(48, 24)); bitmap.Render(preview);
        using var stream = new MemoryStream(); bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        using var pixels = SkiaSharp.SKBitmap.Decode(stream.ToArray());
        Assert.Equal(new SkiaSharp.SKColor(155, 93, 229), pixels.GetPixel(1, 12));
        Assert.Equal(new SkiaSharp.SKColor(155, 93, 229), pixels.GetPixel(47, 12));
        var screenshot = Environment.GetEnvironmentVariable("MODULE_READER_GAP_SCREENSHOT");
        if (!string.IsNullOrEmpty(screenshot))
        {
            var example = new ModulePreview { CellSize = 22, Data = new PreviewService().Create(
                new(new(84, 28, 1, false), RepeatedTrace(), Enumerable.Range(0, 14).ToArray())) };
            var size = new Size(example.Width, example.Height); example.Measure(size); example.Arrange(new Rect(size));
            using var frame = new RenderTargetBitmap(new PixelSize((int)size.Width, (int)size.Height));
            frame.Render(example); frame.Save(screenshot, PngBitmapEncoderOptions.Default);
        }
    }
}
