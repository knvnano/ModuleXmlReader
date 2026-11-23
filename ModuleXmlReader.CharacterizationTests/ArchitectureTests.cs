using System.Reflection;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Infrastructure;
using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Avalonia;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class ArchitectureTests
{
    [Fact]
    public void InnerLayersHaveNoUiOrPersistenceDependencies()
    {
        var domain = typeof(ModuleEditorState).Assembly;
        var application = typeof(ModuleEditorService).Assembly;
        var presentation = typeof(MainWindowViewModel).Assembly;
        foreach (var assembly in new[] { domain, application, presentation })
        {
            var names = assembly.GetReferencedAssemblies().Select(a => a.Name!).ToArray();
            Assert.DoesNotContain(names, n => n.StartsWith("Avalonia") || n.Contains("Windows.Forms") || n == "System.Xml.XDocument");
            Assert.DoesNotContain("ModuleXmlReader.Infrastructure", names);
        }
        Assert.DoesNotContain(domain.GetReferencedAssemblies(), a => a.Name!.StartsWith("ModuleXmlReader."));
        Assert.DoesNotContain(application.GetReferencedAssemblies(), a => a.Name == "ModuleXmlReader.Presentation");
        Assert.DoesNotContain(typeof(XmlModuleRepository).Assembly.GetReferencedAssemblies(), a => a.Name == "ModuleXmlReader.Presentation");
    }
    [Fact]
    public void Stage1SessionIsOnlyInTestsAndViewsHaveNoOperationHandlers()
    {
        Assert.Null(typeof(MainWindow).Assembly.GetType("ModuleXmlReader.Avalonia.LegacySession"));
        Assert.DoesNotContain(typeof(MainWindow).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly),
            m => m.Name is "Open_Click" or "Save_Click" or "Left_Click" or "Right_Click");
    }
}
