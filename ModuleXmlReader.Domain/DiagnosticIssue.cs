namespace ModuleXmlReader.Domain;
public sealed record DiagnosticIssue(string Severity,MessageToken Message,string Field,int? PointNumber=null);
