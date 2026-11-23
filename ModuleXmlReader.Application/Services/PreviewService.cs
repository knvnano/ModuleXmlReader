using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;
public sealed class PreviewService : IPreviewService
{
    public PreviewData Create(ValidatedModule module)
    {
        var g = module.Geometry;
        var points = new List<PreviewPoint>();
        var pixels = new List<PreviewPixel>();
        var inferred = InferStraightTrace(module.Points, g);
        var unlocated = new List<int>();
        for (int i = 0; i < module.Points.Count; i++)
            if (module.Points[i].IsMissing && inferred is null) unlocated.Add(i + 1);
        for (int group = 0; group < g.DataGroups; group++)
            for (int i = 0; i < module.Points.Count; i++)
            {
                var original = module.Points[i];
                if (original.IsMissing && inferred is null) continue;
                var p = original.IsMissing ? inferred![i] : original;
                int x = p.X + (g.IsVertical ? group * g.GroupWidth : 0);
                int y = p.Y + (g.IsVertical ? 0 : group * g.GroupHeight);
                pixels.Add(new(x, y, i + 1, original.IsMissing, original.IsMissing));
                if (!original.IsMissing) points.Add(new(x, y));
            }
        // NovaLCT marks scan addresses at the start of the first group's running
        // line. Index determines A..P,16..; address values determine position.
        var scanLine = new List<PreviewScanAddress>();
        var addresses = module.ScanAddresses;
        var anchor = inferred is not null ? inferred[0] : module.Points.FirstOrDefault(p => !p.IsMissing);
        if (addresses is { Count: > 0 } && module.Points.Any(p => !p.IsMissing))
            for (int i = 0; i < addresses.Count; i++)
            {
                int offset = addresses[i] - addresses[0];
                int x = anchor.X + (g.IsVertical ? offset : 0);
                int y = anchor.Y + (g.IsVertical ? 0 : offset);
                if (x >= 0 && x < g.GroupWidth && y >= 0 && y < g.GroupHeight)
                    scanLine.Add(new(x, y, i));
            }
        return new(g.Cols, g.Rows, points.AsReadOnly(), scanLine.AsReadOnly(), pixels.AsReadOnly(), unlocated.AsReadOnly(),
            CreateGapMarkers(module).AsReadOnly());
    }
    private static List<PreviewGapMarker> CreateGapMarkers(ValidatedModule module)
    {
        var result = new List<PreviewGapMarker>();
        var points = module.Points;
        var g = module.Geometry;
        for (int first = 0; first < points.Count; first++)
        {
            if (!points[first].IsMissing) continue;
            int last = first;
            while (last + 1 < points.Count && points[last + 1].IsMissing) last++;
            int previous = first - 1, next = last + 1;
            (int dx, int dy)? between = previous >= 0 && next < points.Count
                ? Direction(points[previous], points[next]) : null;
            if (previous >= 0)
            {
                var direction = between ?? (previous > 0 && !points[previous - 1].IsMissing
                    ? Direction(points[previous - 1], points[previous]) : null) ?? (g.IsVertical ? (0, 1) : (1, 0));
                Add(previous, Leaving(direction));
            }
            if (next < points.Count)
            {
                var direction = between ?? (next + 1 < points.Count && !points[next + 1].IsMissing
                    ? Direction(points[next], points[next + 1]) : null) ?? (g.IsVertical ? (0, 1) : (1, 0));
                Add(next, Opposite(Leaving(direction)));
            }
            first = last;

            void Add(int index, GapEdge edge)
            {
                var p = points[index];
                for (int group = 0; group < g.DataGroups; group++)
                    result.Add(new(p.X + (g.IsVertical ? group * g.GroupWidth : 0),
                        p.Y + (g.IsVertical ? 0 : group * g.GroupHeight), edge, first + 1, last - first + 1));
            }
        }
        return result;
    }
    private static (int dx, int dy)? Direction(PointTableEntry from, PointTableEntry to) =>
        from.Y == to.Y && from.X != to.X ? (Math.Sign(to.X - from.X), 0) :
        from.X == to.X && from.Y != to.Y ? (0, Math.Sign(to.Y - from.Y)) : null;
    private static GapEdge Leaving((int dx, int dy) direction) => direction.dx switch
    { > 0 => GapEdge.Right, < 0 => GapEdge.Left, _ => direction.dy > 0 ? GapEdge.Bottom : GapEdge.Top };
    private static GapEdge Opposite(GapEdge edge) => edge switch
    { GapEdge.Left => GapEdge.Right, GapEdge.Right => GapEdge.Left, GapEdge.Top => GapEdge.Bottom, _ => GapEdge.Top };
    // A 255-255 pair carries no position. Infer it only when all known entries
    // uniquely prove a straight unit-step trace; keep arbitrary gaps unlocated.
    private static PointTableEntry[]? InferStraightTrace(IReadOnlyList<PointTableEntry> points, ModuleGeometry g)
    {
        int first = Enumerable.Range(0, points.Count).FirstOrDefault(i => !points[i].IsMissing, -1);
        if (first < 0) return null;
        var candidates = new List<PointTableEntry[]>();
        foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
        {
            int x0 = points[first].X - first * dx, y0 = points[first].Y - first * dy;
            var path = Enumerable.Range(0, points.Count).Select(i => new PointTableEntry(x0 + i * dx, y0 + i * dy)).ToArray();
            if (path.All(p => p.X >= 0 && p.X < g.GroupWidth && p.Y >= 0 && p.Y < g.GroupHeight) &&
                Enumerable.Range(0, points.Count).All(i => points[i].IsMissing || points[i] == path[i])) candidates.Add(path);
        }
        return candidates.Count == 1 ? candidates[0] : null;
    }
}
