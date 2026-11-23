using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleExportWriter { string Write(string folder,ModuleExportBundle bundle); }
