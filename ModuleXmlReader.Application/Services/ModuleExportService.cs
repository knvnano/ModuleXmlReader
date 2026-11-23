using System.Text;
using System.Net;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;
public sealed class ModuleExportService
{
 public ModuleExportBundle Build(ModuleEditorState state,Func<string,string> text,Func<MessageToken,string> message)
 {
  var data=state.Preview??throw new InvalidOperationException("A valid module preview is required");
  var svg=new StringBuilder();int cell=20,margin=30,w=data.Cols*cell+margin,h=data.Rows*cell+margin;
  svg.Append($"<svg xmlns='http://www.w3.org/2000/svg' width='{w}' height='{h}' viewBox='0 0 {w} {h}'><title>{WebUtility.HtmlEncode(Path.GetFileName(state.OpenedFilePath))}</title><rect width='{w}' height='{h}' fill='#142037'/><g transform='translate(30 30)'>");
  Rect(0,0,data.Cols*cell,data.Rows*cell,"#263247");
  foreach(var p in data.Points)Rect(p.X*cell,p.Y*cell,cell,cell,"#37C49B");
  foreach(var p in data.Pixels??Array.Empty<PreviewPixel>())if(p.IsMissing)Rect(p.X*cell,p.Y*cell,cell,cell,"#9B5DE5");
  var missing=data.Pixels?.Where(p=>p.IsMissing).Select(p=>(p.X,p.Y)).ToHashSet()??[];
  foreach(var p in data.ScanLine??Array.Empty<PreviewScanAddress>())if(!missing.Contains((p.X,p.Y)))Rect(p.X*cell,p.Y*cell,cell,cell,"#FF6376");
  for(int x=0;x<=data.Cols;x++)svg.Append($"<path d='M{x*cell} 0V{data.Rows*cell}' stroke='#3B4960' stroke-width='0.5'/>");
  for(int y=0;y<=data.Rows;y++)svg.Append($"<path d='M0 {y*cell}H{data.Cols*cell}' stroke='#3B4960' stroke-width='0.5'/>");
  for(int x=0;x<data.Cols;x++)Label((x+1).ToString(),x*cell+10,-9,"#70B8FF");
  for(int y=0;y<data.Rows;y++)Label((y+1).ToString(),-15,y*cell+13,"#70B8FF");
  var addresses=data.ScanLine?.Select(p=>(p.X,p.Y)).ToHashSet()??[];
  foreach(var p in data.Pixels??Array.Empty<PreviewPixel>())
   if(p.IsMissing||!addresses.Contains((p.X,p.Y)))Label(p.Number.ToString(),p.X*cell+10,p.Y*cell+14,p.IsMissing?"white":"black",p.Number);
  foreach(var p in data.ScanLine??Array.Empty<PreviewScanAddress>())if(!missing.Contains((p.X,p.Y)))Label(p.Index<16?((char)('A'+p.Index)).ToString():p.Index.ToString(),p.X*cell+10,p.Y*cell+14,"black");
  foreach(var gap in data.GapMarkers??Array.Empty<PreviewGapMarker>())
  {
   int x=gap.X*cell,y=gap.Y*cell;
   if(gap.Edge==GapEdge.Left)Rect(x,y,3,cell,"#9B5DE5");else if(gap.Edge==GapEdge.Right)Rect(x+cell-3,y,3,cell,"#9B5DE5");else if(gap.Edge==GapEdge.Top)Rect(x,y,cell,3,"#9B5DE5");else Rect(x,y+cell-3,cell,3,"#9B5DE5");
  }
  svg.Append("</g></svg>");
  var report=new StringBuilder().AppendLine("# "+text("ExportReport")).AppendLine().AppendLine(state.OpenedFilePath).AppendLine().AppendLine("## "+text("Parameters"));
  foreach(var name in ModuleParameters.Names)report.AppendLine($"- {name}: {state.Parameters[name]}");
  report.AppendLine().AppendLine("## "+text("Missing")).AppendLine(string.Join(", ",new PointTableParser().Parse(state.PointTableData).Select((p,i)=>(p,i)).Where(a=>a.p.IsMissing).Select(a=>(a.i+1).ToString()))).AppendLine().AppendLine("## "+text("Diagnostics"));
  foreach(var issue in new ModuleDiagnosticService().Analyze(state))report.AppendLine($"- {text(issue.Severity)}: {message(issue.Message)} ({issue.Field})");
  foreach(var field in new[]{("PointTableData",state.PointTableData),("NewScanABCDCode",state.NewScanABCDCode),("ScanABCDCode",state.ScanABCDCode)})report.AppendLine().AppendLine("## "+field.Item1).AppendLine().AppendLine(field.Item2);
  var csv=new StringBuilder("Number,X,Y,Missing,Inferred\r\n");
  foreach(var p in data.Pixels??Array.Empty<PreviewPixel>())csv.AppendLine($"{p.Number},{p.X},{p.Y},{p.IsMissing},{p.IsInferred}");
  foreach(var n in data.UnlocatedMissing??Array.Empty<int>())csv.AppendLine($"{n},,,True,False");
  return new(svg.ToString(),report.ToString(),csv.ToString());
  void Rect(int x,int y,int width,int height,string fill)=>svg.Append($"<rect x='{x}' y='{y}' width='{width}' height='{height}' fill='{fill}'/>");
  void Label(string value,int x,int y,string fill,int? number=null)=>svg.Append($"<text x='{x}' y='{y}' text-anchor='middle' font-family='sans-serif' font-size='9' fill='{fill}' data-point-number='{number}'>{WebUtility.HtmlEncode(value)}</text>");
 }
}
