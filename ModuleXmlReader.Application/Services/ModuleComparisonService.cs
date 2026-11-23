using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;
public sealed class ModuleComparisonService(IModuleRepository repository) : IModuleComparisonService
{
 public IReadOnlyList<ModuleDifference> CompareFile(ModuleEditorState current,string path)
 {
  var doc=repository.Load(path);if(!doc.HasModule)throw new FormatException("Config/Module is absent");
  var before=new ModuleEditorState();foreach(var key in ModuleParameters.Names)before.Parameters[key]=doc.GetValue(key)??"";
  before.PointTableData=doc.GetValue("PointTableData")??"";before.NewScanABCDCode=doc.GetValue("NewScanABCDCode")??"";before.ScanABCDCode=doc.GetValue("ScanABCDCode")??"";
  return Compare(before,current);
 }
 public IReadOnlyList<ModuleDifference> Compare(ModuleEditorState before,ModuleEditorState after)
 {
  var result=new List<ModuleDifference>();
  foreach(var key in ModuleParameters.Names.Where(k=>k!="ScanABCDCode"))
   if(before.Parameters[key]!=after.Parameters[key]) result.Add(new("DiffParameter",key,before.Parameters[key],after.Parameters[key]));
  var parser=new PointTableParser();
  try
  {
   var a=parser.Parse(before.PointTableData);var b=parser.Parse(after.PointTableData);
   for(int i=0;i<Math.Max(a.Count,b.Count);i++)
   {
    var x=i<a.Count?$"{a[i].X}-{a[i].Y}":"—";var y=i<b.Count?$"{b[i].X}-{b[i].Y}":"—";
    if(x==y)continue;
    result.Add(new("DiffPoint",(i+1).ToString(),x,y,i+1));
    bool am=i<a.Count&&a[i].IsMissing,bm=i<b.Count&&b[i].IsMissing;
    if(am!=bm)result.Add(new("DiffMissing",(i+1).ToString(),am?"255-255":"—",bm?"255-255":"—",i+1));
   }
  }
  catch(FormatException) { if(before.PointTableData!=after.PointTableData)result.Add(new("DiffPoint","PointTableData",before.PointTableData,after.PointTableData)); }
  Codes("NewScanABCDCode",before.NewScanABCDCode,after.NewScanABCDCode);
  Codes("ScanABCDCode",before.ScanABCDCode,after.ScanABCDCode);
  return result.AsReadOnly();
  void Codes(string name,string a,string b)
  {
   var x=a.Split('-');var y=b.Split('-');
   for(int i=0;i<Math.Max(x.Length,y.Length);i++)
   { var av=i<x.Length?x[i].Trim():"—";var bv=i<y.Length?y[i].Trim():"—";if(av!=bv)result.Add(new("DiffCode",$"{name}[{i}]",av,bv)); }
  }
 }
}
