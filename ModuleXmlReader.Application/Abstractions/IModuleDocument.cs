namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleDocument
{
    bool HasModule { get; }
    string? GetValue(string name);
    void SetValue(string name, string value);
    IModuleDocument Clone();
}
