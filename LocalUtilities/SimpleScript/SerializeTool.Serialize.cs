using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using LocalUtilities.SimpleScript.Formatter;

namespace LocalUtilities.SimpleScript;

partial class SerializeTool
{
    private static bool SerializeSimpleType(
        [NotNullWhen(false)] object? obj,
        [NotNullWhen(true)] out Func<object?, string>? convert
    )
    {
        convert = null;
        if (obj is null)
        {
            convert = _ => "";
            return true;
        }
        var type = obj.GetType();
        if (TypeConverts.TryGetValue(type, out var c))
            convert = c.ToString;
        else if (typeof(Enum).IsAssignableFrom(type))
            convert = o => o?.ToString() ?? "";
        else
            return false;
        return true;
    }

    private static void Serialize(object? obj, SsWriter writer)
    {
        if (SerializeSimpleType(obj, out var convert))
        {
            writer.AppendValue(convert(obj));
            return;
        }
        var type = obj.GetType();
        writer.AppendStart();
        if (typeof(IDictionary).IsAssignableFrom(type))
            DoDictionanry(obj, writer);
        else if (typeof(ICollection).IsAssignableFrom(type))
            DoList(obj, writer);
        else
            DoOtherType(obj, writer, type);
        writer.AppendEnd();
    }

    private static void DoDictionanry(object obj, SsWriter writer)
    {
        var enumer = ((IDictionary)obj).GetEnumerator();
        using var _ = enumer as IDisposable;
        for (var i = 0; i < ((IDictionary)obj).Count; i++)
        {
            enumer.MoveNext();
            Serialize(enumer.Key, writer);
            Serialize(enumer.Value, writer);
        }
    }

    private static void DoList(object obj, SsWriter writer)
    {
        var enumer = ((ICollection)obj).GetEnumerator();
        using var _ = enumer as IDisposable;
        writer.ValueArray = true;
        for (var i = 0; i < ((ICollection)obj).Count; i++)
        {
            enumer.MoveNext();
            Serialize(enumer.Current, writer);
        }
    }

    private static void DoOtherType(object obj, SsWriter writer, Type type)
    {
        foreach (var property in type.GetProperties(Authority))
        {
            var notSsItem =
                property.GetCustomAttribute<SsIgnore>() is not null || property.SetMethod is null;
            if (notSsItem)
                continue;
            var subObj = property.GetValue(obj, Authority, null, null, null);
            if (subObj is ICollection collection && collection.Count is 0)
                continue;
            writer.AppendName(property.Name);
            Serialize(subObj, writer);
        }
    }

    public static byte[] Serialize(object? obj, Encoding? encoding = null)
    {
        using var memory = new MemoryStream();
        using var writer = new SsStreamWriter(memory, false, encoding ?? Encoding.UTF8);
        if (SerializeSimpleType(obj, out var convert))
            writer.AppendUnquotedValue(convert(obj));
        else
            Serialize(obj, writer);
        var buffer = new byte[memory.Position];
        Array.Copy(memory.GetBuffer(), 0, buffer, 0, buffer.Length);
        return buffer;
    }

    public static string Serialize(object? obj, bool writeIntoMultiLines)
    {
        var writer = new SsStringWriter(writeIntoMultiLines);
        if (SerializeSimpleType(obj, out var convert))
            writer.AppendUnquotedValue(convert(obj));
        else
            Serialize(obj, writer);
        return writer.ToString();
    }

    public static void SerializeFile(object? obj, bool writeIntoMultiLines, string filePath)
    {
        using var file = File.Create(filePath);
        file.Write(Utf8Bom);
        using var writer = new SsStreamWriter(file, writeIntoMultiLines, Encoding.UTF8);
        if (SerializeSimpleType(obj, out var convert))
            writer.AppendUnquotedValue(convert(obj));
        else
            Serialize(obj, writer);
    }
}
