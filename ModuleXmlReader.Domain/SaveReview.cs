namespace ModuleXmlReader.Domain;
public sealed record SaveReview(IReadOnlyList<ModuleDifference> Differences)
{
 public int ParameterCount=>Differences.Count(d=>d.Category=="DiffParameter");
 public int PointCount=>Differences.Where(d=>d.Category=="DiffPoint"&&d.PointNumber.HasValue).Select(d=>d.PointNumber).Distinct().Count();
 public int NewMissingCount=>Differences.Count(d=>d.Category=="DiffMissing"&&d.After=="255-255");
}
