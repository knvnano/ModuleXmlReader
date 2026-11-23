using System.Xml.Linq;
using ModuleXmlReader.Application.Abstractions;
namespace ModuleXmlReader.Infrastructure;

public sealed class XmlModuleSerializer : IModuleSerializer
{
    public IModuleDocument Deserialize(string path) => new XmlModuleDocument(XDocument.Load(path));
    public void Serialize(IModuleDocument document, string path) => ((XmlModuleDocument)document).Xml.Save(path);
    private sealed class XmlModuleDocument(XDocument xml) : IModuleDocument
    {
        public XDocument Xml { get; } = xml;
        private XElement? Module => Xml.Element("Config")?.Element("Module");
        public bool HasModule => Module is not null;
        public string? GetValue(string name) => Module?.Element(name)?.Value;
        public void SetValue(string name, string value)
        {
            var module = Module ?? throw new FormatException("Не найден элемент Config/Module.");
            var element = module.Element(name);
            if (element is null) module.Add(new XElement(name, value));
            else element.Value = value;
        }
        public IModuleDocument Clone() => new XmlModuleDocument(new XDocument(Xml));
    }
}
