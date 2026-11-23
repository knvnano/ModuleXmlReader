using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Infrastructure;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class LoggingIntegrationTests : TempTest
{
    [Fact]
    public void EditorRecordsCaughtErrorsValidationAndSuccessfulOperations()
    {
        var logger = new RecordingLogger(); var parser = new PointTableParser(); var scan = new ScanCodeService();
        var validation = new ModuleValidationService(parser, scan);
        var editor = new ModuleEditorService(new XmlModuleRepository(new XmlModuleSerializer()), new MissingPixelService(parser),
            new ModuleRotationService(parser, scan, validation), validation, new PreviewService(), logger);
        Assert.False(editor.Open(Path.Combine(Dir, "missing.module"))); Assert.Single(logger.Errors);
        Assert.IsType<FileNotFoundException>(logger.Errors[0]);
        Assert.True(editor.Open(TestServices.Fixture())); editor.Rotate(Rotation.Right90);
        Assert.True(editor.Save(Path.Combine(Dir, "saved.module")));
        editor.UpdateTables("invalid", editor.State.NewScanABCDCode, editor.State.ScanABCDCode);
        Assert.NotEmpty(logger.Diagnostics);
        Assert.False(editor.Save(Path.Combine(Dir, "invalid.module"))); Assert.Equal(2, logger.Errors.Count);
        Assert.Contains(logger.Events, message => message.Contains("Module saved:"));
        Assert.Contains(logger.Events, message => message.Contains("Module rotated:"));
    }
    private sealed class RecordingLogger : IAppLogger
    {
        public List<string> Events { get; } = new();
        public List<string> Diagnostics { get; } = new();
        public List<Exception> Errors { get; } = new();
        public void Info(string message) => Events.Add(message);
        public void Debug(string message) => Diagnostics.Add(message);
        public void Error(string message, Exception exception) => Errors.Add(exception);
    }
}
