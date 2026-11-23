using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IChangelogReader
{
    IReadOnlyList<ChangeEntry> Read();
}
public interface ILogReader
{
    string DirectoryPath { get; }
    string ReadTail(LogKind kind);
}
