using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Infrastructure;
namespace ModuleXmlReader.CharacterizationTests;
internal static class TestServices
{
    public static string Fixture(int angle = 0) => Path.Combine(AppContext.BaseDirectory, "Fixtures", $"128_64_{(angle == 0 ? "normal" : angle)}.module");
    public static ModuleEditorService CreateEditor(IModuleRepository? repository = null)
    {
        var parser = new PointTableParser(); var scan = new ScanCodeService();
        var validation = new ModuleValidationService(parser, scan);
        return new(repository ?? new XmlModuleRepository(new XmlModuleSerializer()), new MissingPixelService(parser),
            new ModuleRotationService(parser, scan, validation), validation, new PreviewService());
    }
}
public abstract class TempTest : IDisposable
{
    protected string Dir { get; } = Path.Combine(Path.GetTempPath(), "module-reader-fixed-" + Guid.NewGuid());
    protected TempTest() => Directory.CreateDirectory(Dir);
    public void Dispose() => Directory.Delete(Dir, recursive: true);
}
