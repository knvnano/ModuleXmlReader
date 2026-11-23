using System.Text;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Infrastructure;
public sealed class ModuleExportWriter : IModuleExportWriter
{
 public string Write(string folder,ModuleExportBundle bundle)
 {
  string output=Path.Combine(folder,"module-report-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")+"-"+Guid.NewGuid().ToString("N")[..8]);
  Directory.CreateDirectory(output);
  File.WriteAllText(Path.Combine(output,"trace.svg"),bundle.Svg,new UTF8Encoding(false));
  File.WriteAllText(Path.Combine(output,"report.md"),bundle.Report,new UTF8Encoding(false));
  File.WriteAllText(Path.Combine(output,"points.csv"),bundle.PointsCsv,new UTF8Encoding(true));
  return output;
 }
}
