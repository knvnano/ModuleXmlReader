using System.Text.Json;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Infrastructure;

public sealed class JsonChangelogReader(string path) : IChangelogReader
{
    public IReadOnlyList<ChangeEntry> Read()
    {
        if (!File.Exists(path)) return [];
        using var stream = File.OpenRead(path);
        var entries = JsonSerializer.Deserialize<List<ChangeEntry>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new FormatException("The changelog root must be an array.");
        if (entries.Any(e => e is null || string.IsNullOrWhiteSpace(e.Version) || e.TitleRu is null || e.TitleEn is null ||
            e.ItemsRu is null || e.ItemsEn is null || e.ItemsRu.Any(i => i is null) || e.ItemsEn.Any(i => i is null)))
            throw new FormatException("The changelog contains an incomplete entry.");
        return entries.AsReadOnly();
    }
}
