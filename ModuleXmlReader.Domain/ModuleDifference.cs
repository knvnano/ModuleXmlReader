namespace ModuleXmlReader.Domain;
public sealed record ModuleDifference(string Category,string Name,string Before,string After,int? PointNumber=null);
