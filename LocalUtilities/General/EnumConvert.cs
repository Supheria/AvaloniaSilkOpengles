using System.ComponentModel;
using EnumsNET;

namespace LocalUtilities.General;

public sealed class EnumConvert
{
    public static object ToEnum(string str, Type enumType)
    {
        return Enums.Parse(enumType, str, true);
    }

    public static object? DescriptionToEnum(string? str, Type type)
    {
        if (str is null)
            return null;
        var map = new Dictionary<string, string>();
        var fieldinfos = type.GetFields();
        foreach (var field in fieldinfos)
        {
            var atts = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (atts.Length is 0)
                continue;
            map[((DescriptionAttribute)atts[0]).Description] = field.Name;
        }
        return !map.TryGetValue(str, out var e) ? null : ToEnum(e, type);
    }

    public static object? ToEnumItem(int index, Type enumType)
    {
        var names = Enums.GetNames(enumType);
        if (index >= names.Count || index < 0)
            return null;
        var obj = ToEnum(names[index], enumType);
        return obj;
    }
}
