using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed partial class MainWindowViewModel
{
 private IReadOnlyList<DiagnosticViewModel> _diagnostics=Array.Empty<DiagnosticViewModel>();
 public IReadOnlyList<DiagnosticViewModel> Diagnostics {get=>_diagnostics;private set=>SetProperty(ref _diagnostics,value);}
 private void RefreshDiagnostics() => Diagnostics=new ModuleDiagnosticService().Analyze(_editor.State).Select(d=>new DiagnosticViewModel(Text[d.Severity],Text.Format(d.Message),d.Field,new RelayCommand(()=> { if(d.PointNumber is int n) Navigation.Select(n);else Navigation.SelectField(d.Field); }))).ToArray();
}
public sealed record DiagnosticViewModel(string Severity,string Message,string Field,RelayCommand GoCommand);
