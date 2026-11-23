using ModuleXmlReader.Domain;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class UndoTests : TempTest
{
 [Fact] public void UndoRestoresInvalidEditsRotationAndSavedSignature()
 {
  var e=TestServices.CreateEditor();Assert.True(e.Open(TestServices.Fixture()));var original=e.State.PointTableData;
  e.UpdateTables("bad",e.State.NewScanABCDCode,e.State.ScanABCDCode);Assert.Null(e.State.Preview);
  e.Undo();Assert.Equal(original,e.State.PointTableData);Assert.False(e.State.HasChanges);
  e.Redo();Assert.Equal("bad",e.State.PointTableData);e.Undo();
  e.Rotate(Rotation.Right90);var rotated=e.State.PointTableData;Assert.False(e.CanRedo);
  var path=Path.Combine(Dir,"saved.module");Assert.True(e.Save(path));e.Undo();Assert.True(e.State.HasChanges);Assert.Equal(path,e.State.OpenedFilePath);
  e.Redo();Assert.Equal(rotated,e.State.PointTableData);Assert.False(e.State.HasChanges);
  Assert.True(e.Open(TestServices.Fixture()));Assert.False(e.CanUndo);Assert.False(e.CanRedo);
 }
}
