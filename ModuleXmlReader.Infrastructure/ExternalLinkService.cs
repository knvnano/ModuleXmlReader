using System.Diagnostics;
using ModuleXmlReader.Application.Abstractions;
namespace ModuleXmlReader.Infrastructure;

public sealed class ExternalLinkService : IExternalLinkService
{
    public void Open(string url) => Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
}
