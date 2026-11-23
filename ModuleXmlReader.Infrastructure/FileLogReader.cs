using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Infrastructure;

public sealed class FileLogReader(string directory) : ILogReader
{
    public string DirectoryPath { get; } = Path.GetFullPath(directory);
    public string ReadTail(LogKind kind)
    {
        string file = kind switch { LogKind.Activity => "activity.log", LogKind.Errors => "errors.log", LogKind.Diagnostics => "diagnostics.log", _ => throw new ArgumentOutOfRangeException(nameof(kind)) };
        string path = Path.Combine(DirectoryPath, file);
        if (!File.Exists(path)) return string.Empty;
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        bool truncated = stream.Length > 128 * 1024;
        if (truncated) stream.Seek(-128 * 1024, SeekOrigin.End);
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        if (truncated) reader.ReadLine();
        return reader.ReadToEnd();
    }
}
