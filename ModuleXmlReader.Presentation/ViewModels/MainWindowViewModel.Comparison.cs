using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed partial class MainWindowViewModel
{
 private readonly IModuleComparisonService? _comparison;
 private IReadOnlyList<DifferenceViewModel> _differences=Array.Empty<DifferenceViewModel>();
 private PreviewData? _comparedPreview;
 private string _comparisonPath="";
 public string ComparisonPath { get=>_comparisonPath;private set=>SetProperty(ref _comparisonPath,value); }
 public IReadOnlyList<DifferenceViewModel> Differences { get=>_differences;private set=>SetProperty(ref _differences,value); }
 public AsyncCommand CompareCommand { get; }
 private void RefreshComparison()
 {
  if(!ReferenceEquals(_comparedPreview,Preview)) { Differences=Array.Empty<DifferenceViewModel>();ComparisonPath="";_comparedPreview=Preview; }
  else Differences=Differences.Select(d=>new DifferenceViewModel(d.Difference,Text[d.Difference.Category])).ToArray();
 }
 private async Task CompareAsync()
 {
  var path=await _files.PickOpenFileAsync();if(path is null || _comparison is null)return;
  var items=await Task.Run(()=>_comparison.CompareFile(_editor.State,path));
  _comparedPreview=Preview; ComparisonPath=path; Differences=items.Select(d=>new DifferenceViewModel(d,Text[d.Category])).ToArray();
  _log.Info($"Compared with {path}: {items.Count} differences");
 }
}
public sealed record DifferenceViewModel(ModuleDifference Difference,string Category)
{
 public string Name=>Difference.Name;
 public string Before=>Difference.Before;
 public string After=>Difference.After;
}
