namespace ModuleXmlReader.Domain;
public sealed record BatchResult(string File,int? RelativeAngle,bool Success,string Output,MessageToken Message);
