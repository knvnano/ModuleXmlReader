using System.Globalization;
using ModuleXmlReader.Application.Abstractions;
namespace ModuleXmlReader.Application.Services;

public sealed class ScanCodeService : IScanCodeService
{
    public int[] Parse(string code, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(code)) return [];
        return code.Split('-').Select((text, i) => byte.TryParse(text.Trim(), NumberStyles.None,
            CultureInfo.InvariantCulture, out var value) ? (int)value :
            throw Messages.Format("ScanByte", $"{fieldName}: значение {i + 1} должно быть числом от 0 до 255." , fieldName, i + 1)).ToArray();
    }
    public string MoveActive(string code,int index,int offset,int activeLength,string fieldName)
    {
        var values=Parse(code,fieldName);
        if(activeLength<0||activeLength>values.Length||index<0||index>=activeLength||index+offset<0||index+offset>=activeLength||Math.Abs(offset)!=1)
            throw new ArgumentOutOfRangeException(nameof(index));
        (values[index],values[index+offset])=(values[index+offset],values[index]);
        return string.Join("-",values);
    }
    public string ReversePrefix(string code, int activeLength, string fieldName)
    {
        var values = Parse(code, fieldName);
        if (activeLength < 0 || activeLength > values.Length)
            throw Messages.Format("ScanShort", $"{fieldName}: недостаточно значений для {activeLength} адресов." , fieldName, activeLength);
        Array.Reverse(values, 0, activeLength);
        return string.Join("-", values);
    }
}
