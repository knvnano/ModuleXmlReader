using Avalonia.Controls;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Avalonia.Services;
public sealed partial class AvaloniaDialogService
{
 public async Task<bool> ConfirmSaveAsync(SaveReview review)
 {
  var window=new Window {Width=850,Height=540,MinWidth=600,MinHeight=350,WindowStartupLocation=WindowStartupLocation.CenterOwner};
  var summary=new TextBlock {TextWrapping=global::Avalonia.Media.TextWrapping.Wrap,FontSize=16};
  var details=new TextBox {IsReadOnly=true,AcceptsReturn=true,TextWrapping=global::Avalonia.Media.TextWrapping.Wrap,IsVisible=false};
  var expand=new Button();expand.Click+=(_,_)=>details.IsVisible=!details.IsVisible;
  var save=new Button {IsDefault=true};var cancel=new Button {IsCancel=true};save.Click+=(_,_)=>window.Close(true);cancel.Click+=(_,_)=>window.Close(false);
  var buttons=new StackPanel {Orientation=global::Avalonia.Layout.Orientation.Horizontal,Spacing=10,Children={expand,save,cancel}};
  var grid=new Grid {RowDefinitions=new("Auto,*,Auto"),RowSpacing=16,Margin=new(22)};grid.Children.Add(summary);Grid.SetRow(details,1);grid.Children.Add(details);Grid.SetRow(buttons,2);grid.Children.Add(buttons);window.Content=grid;
  void Update(object? sender,EventArgs e)
  {
   window.Title=_text["SaveReviewTitle"];summary.Text=_text.Format("SaveReviewSummary",review.ParameterCount,review.PointCount,review.NewMissingCount,review.Differences.Count);
   expand.Content=_text["DetailedComparison"];save.Content=_text["Save"];cancel.Content=_text["Cancel"];
   details.Text=string.Join(Environment.NewLine,review.Differences.Select(d=>$"{_text[d.Category]} {d.Name}: {d.Before} → {d.After}"));
  }
  Update(null,EventArgs.Empty);_text.LanguageChanged+=Update;
  try {return await window.ShowDialog<bool>(owner);} finally {_text.LanguageChanged-=Update;}
 }
}
