namespace ModuleXmlReader.Application.Abstractions;
public interface IAppLogger
{
    void Info(string message);
    void Debug(string message);
    void Error(string message, Exception exception);
}
public sealed class NullAppLogger : IAppLogger
{
    public static NullAppLogger Instance { get; } = new();
    public void Info(string message) { }
    public void Debug(string message) { }
    public void Error(string message, Exception exception) { }
}
