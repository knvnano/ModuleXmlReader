using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Services;
using ModuleXmlReader.Presentation.ViewModels;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class SaveReviewTests : TempTest
{
 [Fact] public async Task RejectedReviewDoesNotWriteAndSuccessfulSaveResetsBaseline()
 {
  var e=TestServices.CreateEditor();var d=new FakeDialogs {OpenPath=TestServices.Fixture(),SavePath=Path.Combine(Dir,"review.module")};var review=new Review();using var vm=new MainWindowViewModel(e,d,d,d,d,saveReview:review);
  await vm.OpenCommand.ExecuteAsync();var points=vm.PointTableData.Split('-');points[0]=points[1]="255";vm.PointTableData=string.Join("-",points);
  await vm.SaveCommand.ExecuteAsync();Assert.False(File.Exists(d.SavePath));Assert.Equal(1,review.Last!.NewMissingCount);
  review.Accept=true;await vm.SaveCommand.ExecuteAsync();Assert.True(File.Exists(d.SavePath));Assert.Empty(e.GetSaveDifferences());
  e.Undo();Assert.NotEmpty(e.GetSaveDifferences());
 }
 private class Review : ISaveReviewService {public bool Accept;public SaveReview? Last;public Task<bool> ConfirmSaveAsync(SaveReview review) {Last=review;return Task.FromResult(Accept);}}
}
