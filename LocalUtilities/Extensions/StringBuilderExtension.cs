using System.Text;

namespace LocalUtilities.Extensions;

public static class StringBuilderExtension
{
    public static StringBuilder AppendJoin<T>(
        this StringBuilder sb,
        char separator,
        IList<T> source,
        Action<StringBuilder, T> func
    )
    {
        if (source.Count is 0)
            return sb;
        if (source[0] is not null)
            func(sb, source[0]);
        for (var i = 1; i < source.Count; ++i)
        {
            if (separator is not '\0')
                sb.Append(separator);
            if (source[i] is not null)
                func(sb, source[i]);
        }
        return sb;
    }

    public static StringBuilder AppendJoin<T>(
        this StringBuilder sb,
        string separator,
        IList<T> source,
        Action<StringBuilder, T> func
    )
    {
        if (source.Count is 0)
            return sb;
        if (source[0] is not null)
            func(sb, source[0]);
        for (var i = 1; i < source.Count; ++i)
        {
            if (separator is not "")
                sb.Append(separator);
            if (source[i] is not null)
                func(sb, source[i]);
        }
        return sb;
    }
}
