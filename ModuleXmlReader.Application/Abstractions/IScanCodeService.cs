namespace ModuleXmlReader.Application.Abstractions;
public interface IScanCodeService
{
    int[] Parse(string code, string fieldName);
    string ReversePrefix(string code, int activeLength, string fieldName);
}
