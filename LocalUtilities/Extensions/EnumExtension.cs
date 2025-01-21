using System.ComponentModel;

namespace LocalUtilities.Extensions;

public static class EnumExtension
{
    public static string GetDescription<T>(this T @enum)
        where T : Enum
    {
        var name = @enum.ToString();
        var fieldinfos = typeof(T).GetFields();
        foreach (var field in fieldinfos)
        {
            if (field.Name != name)
                continue;
            var atts = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return atts.Length is 0 ? "" : ((DescriptionAttribute)atts[0]).Description;
        }
        return "";
    }

    public static string ToWholeString(this Enum @enum)
    {
        return $"{@enum.GetType().Name}.{@enum}";
    }
}
