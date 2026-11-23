using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;
public sealed class ModuleDiagnosticService
{
 public IReadOnlyList<DiagnosticIssue> Analyze(ModuleEditorState s)
 {
  var issues=new List<DiagnosticIssue>();if(!s.IsLoaded)return issues;
  int cols=Positive("PixelCols"),rows=Positive("PixelRows"),groups=Positive("DataGroup");
  bool vertical=s.Parameters["DataDirectType"]=="Vertical";
  if(s.Parameters["DataDirectType"] is not ("Vertical" or "Horizontal")) Add("SeverityError","DirectionInvalid","DataDirectType");
  if(groups>8)Add("SeverityError","GroupRange","DataGroup");
  int width=cols,height=rows;
  if(groups>0)
  {
   if((vertical?cols:rows)%groups!=0)Add("SeverityError","GroupDivision","DataGroup");
   if(vertical)width/=groups;else height/=groups;
   if(width>255||height>255)Add("SeverityError","GroupSize","DataGroup");
  }
  var raw=s.PointTableData.Split('-');int count=raw.Length/2;
  if(raw.Length%2!=0)Add("SeverityError","DiagOdd","PointTableData");
  var coordinates=new Dictionary<(int,int),int>();
  for(int i=0;i<count;i++)
  {
   int number=i+1;
   if(!byte.TryParse(raw[i*2].Trim(),out byte x)||!byte.TryParse(raw[i*2+1].Trim(),out byte y)) {Add("SeverityError","DiagByte","PointTableData",number,number);continue;}
   if((x==255)!=(y==255)) {Add("SeverityError","DiagHalfMissing","PointTableData",number,number);continue;}
   if(x==255) {Add("SeverityInfo","DiagMissing","PointTableData",number,number);continue;}
   if(x>=width||y>=height)Add("SeverityError","PointBounds","PointTableData",number,number,x,y,width,height);
   if(coordinates.TryGetValue((x,y),out int prior))Add("SeverityWarning","DiagDuplicate","PointTableData",number,prior,number);
   else coordinates[(x,y)]=number;
  }
  var total=s.Parameters["TotalPointInTable"];
  if(!int.TryParse(total,out int declared)||declared!=count)Add("SeverityError","PointCount","PointTableData",null,total,count);
  var scan=new ScanCodeService();
  foreach(var item in new[]{("NewScanABCDCode",s.NewScanABCDCode),("ScanABCDCode",s.ScanABCDCode)})
  {
   try {
    var values=scan.Parse(item.Item2,item.Item1);
    if(item.Item1=="NewScanABCDCode" && values.Length>0 && int.TryParse(s.Parameters["ScanType"].Replace("Scan_",""),out int active) && active>values.Length)Add("SeverityError","NewScanShort",item.Item1,null,active);
   }
   catch(FormatException ex) {issues.Add(new("SeverityError",Messages.FromException(ex),item.Item1));}
  }
  if(s.Preview?.Simulation?.Problem is { } problem)issues.Add(new("SeverityWarning",problem,"DataGroupSequence"));
  return issues.AsReadOnly();
  int Positive(string key) {if(int.TryParse(s.Parameters[key],out int value)&&value>0)return value;Add("SeverityError","PositiveInteger",key,null,key);return 0;}
  void Add(string severity,string key,string field,int? point=null,params object[] args)=>issues.Add(new(severity,new(key,key,args),field,point));
 }
}
