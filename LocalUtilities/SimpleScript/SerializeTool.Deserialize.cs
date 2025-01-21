using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using LocalUtilities.General;
using LocalUtilities.SimpleScript.Parser;

namespace LocalUtilities.SimpleScript;

partial class SerializeTool
{
    private static bool DeserializeSimpleType(
        Type type,
        [NotNullWhen(true)] out Func<string, object?>? convert
    )
    {
        convert = null;
        if (TypeConverts.TryGetValue(type, out var c))
            convert = c.ToType;
        else if (typeof(Enum).IsAssignableFrom(type))
            convert = str => EnumConvert.ToEnum(str, type);
        else
            return false;
        return true;
    }

    private static object? Deserialize(Type type, in Tokenizer tokenizer)
    {
        tokenizer.NextToken();
        if (tokenizer.Token.Type is TokenType.Tail)
            return null;
        if (DeserializeSimpleType(type, out var convert))
            return DoSimpleType(tokenizer, convert);
        if (typeof(IDictionary).IsAssignableFrom(type))
            return DoDictionary(type, tokenizer);
        if (typeof(ICollection).IsAssignableFrom(type))
            return DoList(type, tokenizer);
        return DoOtherType(type, tokenizer);
    }

    private static object? DoSimpleType(Tokenizer tokenizer, Func<string, object?> convert)
    {
        object? obj = null;
        if (tokenizer.Token.Type is TokenType.Value)
            obj = convert(tokenizer.Token.Text);
        tokenizer.NextToken();
        return obj;
    }

    private static object? DoDictionary(Type type, Tokenizer tokenizer)
    {
        var openType = typeof(Dictionary<,>);
        var pairType = type.GetGenericArguments();
        var closeType = openType.MakeGenericType(pairType[0], pairType[1]);
        var obj = Activator.CreateInstance(closeType);
        if (obj is null)
            return null;
        var add = type.GetMethod("Add");
        while (tokenizer.Token.Type is not TokenType.Tail)
        {
            var key = Deserialize(pairType[0], tokenizer);
            if (key is null)
                continue;
            var value = Deserialize(pairType[1], tokenizer);
            if (value is not null)
                add?.Invoke(obj, [key, value]);
        }
        tokenizer.NextToken();
        return obj;
    }

    private static object? DoList(Type type, Tokenizer tokenizer)
    {
        var openType = typeof(List<>);
        var itemType = type.GetGenericArguments()[0];
        var closeType = openType.MakeGenericType(itemType);
        var obj = Activator.CreateInstance(closeType);
        if (obj is null)
            return null;
        var add = type.GetMethod("Add");
        while (tokenizer.Token.Type is not TokenType.Tail)
        {
            var itemObj = Deserialize(itemType, tokenizer);
            if (itemObj is not null)
                add?.Invoke(obj, [itemObj]);
        }
        tokenizer.NextToken();
        return obj;
    }

    private static object? DoOtherType(Type type, Tokenizer tokenizer)
    {
        var obj = Activator.CreateInstance(type);
        if (obj is null)
            return null;
        while (tokenizer.Token.Type is not TokenType.Tail)
        {
            if (tokenizer.Token.Type is not TokenType.Value)
            {
                tokenizer.NextToken();
                continue;
            }
            var property = type.GetProperty(tokenizer.Token.Text, Authority);
            if (property is null)
                throw SsParseException.PropertyNameMismatch(type, tokenizer.Token.Text);
            var subObj = Deserialize(property.PropertyType, tokenizer);
            if (subObj is not null)
                property.SetValue(obj, subObj, Authority, null, null, null);
        }
        tokenizer.NextToken();
        return obj;
    }

    public static object? Deserialize(
        Type type,
        byte[] buffer,
        int offset,
        int count,
        Encoding? encoding = null
    )
    {
        if (HasBom(buffer))
        {
            offset += 3;
            count -= 3;
            if (count < 0)
                count = 0;
        }
        encoding ??= Encoding.UTF8;
        if (DeserializeSimpleType(type, out var convert))
        {
            var str = encoding.GetString(buffer, offset, count);
            return convert(str);
        }
        var tokenizer = new Tokenizer(buffer, offset, count, encoding);
        return Deserialize(type, tokenizer);
    }

    public static object? Deserialize(Type type, string str)
    {
        if (DeserializeSimpleType(type, out var convert))
            return convert(str);
        var tokenizer = new Tokenizer(str);
        return Deserialize(type, tokenizer);
    }

    public static T? Deserialize<T>(byte[] buffer, int offset, int count, Encoding? encoding = null)
    {
        return (T?)Deserialize(typeof(T), buffer, offset, count, encoding);
    }

    public static T? Deserialize<T>(string str)
    {
        return (T?)Deserialize(typeof(T), str);
    }

    public static T? DeserializeFile<T>(string filePath)
    {
        var buffer = ReadFileBuffer(filePath);
        return (T?)Deserialize(typeof(T), buffer, 0, buffer.Length, Encoding.UTF8);
    }

    public static object? DeserializeFile(Type type, string filePath)
    {
        var buffer = ReadFileBuffer(filePath);
        return Deserialize(type, buffer, 0, buffer.Length, Encoding.UTF8);
    }

    private static byte[] ReadFileBuffer(string filePath)
    {
        if (!File.Exists(filePath))
            throw SsParseException.CannotOpenFile(filePath);
        using var file = File.OpenRead(filePath);
        var buffer = new byte[file.Length];
        _ = file.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    private static bool HasBom(byte[] buffer)
    {
        return buffer[0] == Utf8Bom[0] && buffer[1] == Utf8Bom[1] && buffer[2] == Utf8Bom[2];
    }
}
