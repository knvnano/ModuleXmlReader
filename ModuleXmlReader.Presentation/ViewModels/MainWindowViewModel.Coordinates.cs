using ModuleXmlReader.Application.Services;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed partial class MainWindowViewModel
{
 private IReadOnlyList<CoordinateRowViewModel> _coordinates=Array.Empty<CoordinateRowViewModel>();
 private CoordinateRowViewModel? _selectedCoordinate;
 private string _coordinateRaw=""; private string _coordinateLanguage="";
 public IReadOnlyList<CoordinateRowViewModel> Coordinates {get=>_coordinates;private set=>SetProperty(ref _coordinates,value);}
 public CoordinateRowViewModel? SelectedCoordinate
 {
  get=>_selectedCoordinate;
  set { if(SetProperty(ref _selectedCoordinate,value) && value is not null && Navigation.SelectedNumber!=value.Number) Navigation.Select(value.Number); }
 }
 private void RefreshCoordinates()
 {
  if(_coordinateRaw==PointTableData && _coordinateLanguage==Text.Language)return;
  _coordinateRaw=PointTableData; _coordinateLanguage=Text.Language;
  try { Coordinates=new PointTableParser().Parse(PointTableData).Select((p,i)=>new CoordinateRowViewModel(i+1,p.X,p.Y,Text,ApplyCoordinate)).ToArray(); }
  catch(FormatException) { Coordinates=Array.Empty<CoordinateRowViewModel>(); }
  SelectedCoordinate=Coordinates.FirstOrDefault(p=>p.Number==Navigation.SelectedNumber);
 }
 private bool ApplyCoordinate(int number,int x,int y)
 {
  if(IsBusy||!IsLoaded)return false;
  var parser=new PointTableParser();var points=parser.Parse(PointTableData).ToArray();
  if(number<1||number>points.Length)return false;
  // Validate the candidate before changing the document.
  var model=Preview?.Simulation;
  if(x!=255 && model is not null && (x>=model.Geometry.GroupWidth||y>=model.Geometry.GroupHeight))return false;
  points[number-1]=new(x,y); PointTableData=parser.Format(points); Navigation.Select(number);return true;
 }
}
