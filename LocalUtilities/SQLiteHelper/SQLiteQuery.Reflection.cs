using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using LocalUtilities.General;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper.Converts;
using Microsoft.Data.Sqlite;

namespace LocalUtilities.SQLiteHelper;

partial class SqLiteQuery
{
    static BindingFlags Authority { get; } =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    static ConcurrentDictionary<Type, TypeConvert> TypeConverts { get; } =
        new()
        {
            [typeof(byte)] = new ByteConvert(),
            [typeof(char)] = new CharConvert(),
            [typeof(short)] = new ShortConvert(),
            [typeof(int)] = new IntConvert(),
            [typeof(long)] = new LongConvert(),
            [typeof(float)] = new FloatConvert(),
            [typeof(double)] = new DoubleConvert(),
        };

    public static string QuoteName(string name)
    {
        return new StringBuilder()
            .Append(Keywords.DoubleQuote)
            .Append(name)
            .Append(Keywords.DoubleQuote)
            .ToString();
    }

    public static string QuoteValue(string value)
    {
        return new StringBuilder()
            .Append(Keywords.Quote)
            .Append(value)
            .Append(Keywords.Quote)
            .ToString();
    }

    public static string GetConditionsString(Condition?[] conditions, ConditionCombo combo)
    {
        if (conditions.Length < 1)
            return "";
        var comboWord = combo switch
        {
            ConditionCombo.Or => Keywords.Or,
            ConditionCombo.And => Keywords.And,
            _ => Keywords.Or,
        };
        var sb = new StringBuilder();
        var first = true;
        foreach (var condition in conditions)
        {
            if (condition is null)
                continue;
            if (first)
            {
                sb.Append(Keywords.Where);
                first = false;
            }
            else
                sb.Append(comboWord);
            var value = SerializeTool.Serialize(condition.Value, false);
            sb.Append(QuoteName(condition.FieldName))
                .Append(condition.Operate)
                .Append(QuoteValue(value));
        }
        ;
        return sb.ToString();
    }

    private static Keywords ConvertType(Type type)
    {
        if (TypeConverts.TryGetValue(type, out var convert))
            return convert.GetTypeKeyword();
        return Keywords.Text;
    }

    public static bool ConvertType(
        SqliteDataReader reader,
        Type type,
        [NotNullWhen(true)] out Func<SqliteDataReader, int, object>? convert
    )
    {
        if (TypeConverts.TryGetValue(type, out var c))
        {
            convert = c.GetConvert();
            return true;
        }
        convert = null;
        return false;
    }

    private static bool NotField(PropertyInfo property)
    {
        return property.GetCustomAttribute<TableIgnore>() is not null || property.SetMethod is null;
    }

    private static void GetFieldNameInfo(
        PropertyInfo property,
        out string name,
        out bool isPrimaryKey,
        out bool isUnique
    )
    {
        var fieldAtrribute = property.GetCustomAttribute<TableField>();
        name = fieldAtrribute?.Name ?? property.Name;
        if (
            name.Contains(Keywords.Quote.ToString())
            || name.Contains(Keywords.DoubleQuote.ToString())
        )
            name = property.Name;
        isPrimaryKey = fieldAtrribute?.IsPrimaryKey ?? false;
        isUnique = fieldAtrribute?.IsUnique ?? false;
    }

    /// <summary>
    /// get value from <typeparamref name="T"/>'s field of given <paramref name="propertyName"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static FieldName? GetFieldName<T>(string propertyName)
    {
        var type = typeof(T);
        var property = type.GetProperty(propertyName);
        if (property is null || NotField(property))
            return null;
        GetFieldNameInfo(property, out var name, out var isPrimaryKey, out var isUnique);
        return new(name, property, isPrimaryKey, isUnique);
    }

    /// <summary>
    /// get value from <typeparamref name="T"/>'s field which is primary key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static FieldName? GetFieldName<T>()
    {
        var type = typeof(T);
        foreach (var property in type.GetProperties(Authority))
        {
            if (NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out var isUnique);
            if (isPrimaryKey)
                return new(name, property, isPrimaryKey, isUnique);
        }
        return null;
    }

    /// <summary>
    /// get names from all <typeparamref name="T"/>'s fields
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static FieldName[] GetFieldNames<T>()
    {
        var type = typeof(T);
        var fieldNames = new List<FieldName>();
        foreach (var property in type.GetProperties(Authority))
        {
            if (NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out var isUnique);
            fieldNames.Add(new(name, property, isPrimaryKey, isUnique));
        }
        return fieldNames.ToArray();
    }

    /// <summary>
    /// get names from <typeparamref name="T"/>'s fields of given <paramref name="propertyNames"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    public static FieldName[] GetFieldNames<T>(params string[] propertyNames)
    {
        var type = typeof(T);
        var fieldNames = new List<FieldName>();
        foreach (var propertyName in propertyNames)
        {
            var property = type.GetProperty(propertyName);
            if (property is null || NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out var isUnique);
            fieldNames.Add(new(name, property, isPrimaryKey, isUnique));
        }
        return fieldNames.ToArray();
    }

    /// <summary>
    /// get value from <paramref name="obj"/>'s field of given <paramref name="propertyName"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static FieldValue? GetFieldValue(object obj, string propertyName)
    {
        var type = obj.GetType();
        var property = type.GetProperty(propertyName);
        if (property is null || NotField(property))
            return null;
        GetFieldNameInfo(property, out var name, out var isPrimaryKey, out _);
        return new(name, property.GetValue(obj), isPrimaryKey);
    }

    /// <summary>
    /// get value from <paramref name="obj"/>'s field which is primary key
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static FieldValue? GetFieldValue(object obj)
    {
        var type = obj.GetType();
        foreach (var property in type.GetProperties(Authority))
        {
            if (NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out _);
            if (isPrimaryKey)
                return new(name, property.GetValue(obj), isPrimaryKey);
        }
        return null;
    }

    /// <summary>
    /// get values from all <paramref name="obj"/>'s fields
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static FieldValue[] GetFieldValues(object obj)
    {
        var type = obj.GetType();
        var fieldValues = new List<FieldValue>();
        foreach (var property in type.GetProperties(Authority))
        {
            if (NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out _);
            fieldValues.Add(new(name, property.GetValue(obj), isPrimaryKey));
        }
        return fieldValues.ToArray();
    }

    /// <summary>
    /// get values from <paramref name="obj"/>'s fields of given <paramref name="propertyNames"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    public static FieldValue[] GetFieldValues(object obj, params string[] propertyNames)
    {
        var type = obj.GetType();
        var fieldValues = new List<FieldValue>();
        foreach (var propertyName in propertyNames)
        {
            var property = type.GetProperty(propertyName);
            if (property is null || NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out _);
            fieldValues.Add(new(name, property.GetValue(obj), isPrimaryKey));
        }
        return fieldValues.ToArray();
    }

    /// <summary>
    /// get <see cref="Condition"/> from <paramref name="obj"/>'s field of given <paramref name="propertyName"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="propertyName"></param>
    /// <param name="operate"></param>
    /// <returns></returns>
    public static Condition? GetCondition(object obj, OperatorType operate, string propertyName)
    {
        var type = obj.GetType();
        var property = type.GetProperty(propertyName);
        if (property is null || NotField(property))
            return null;
        GetFieldNameInfo(property, out var name, out _, out _);
        return new(name, property.Name, property.GetValue(obj), operate);
    }

    /// <summary>
    /// get <see cref="Condition"/> from <paramref name="obj"/>'s field which is primary key
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="operate"></param>
    /// <returns></returns>
    public static Condition? GetCondition(object obj, OperatorType operate)
    {
        var type = obj.GetType();
        foreach (var property in type.GetProperties(Authority))
        {
            if (NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out var isPrimaryKey, out _);
            if (isPrimaryKey)
                return new(name, property.Name, property.GetValue(obj), operate);
        }
        return null;
    }

    /// <summary>
    /// get <see cref="OperatorType.Equal"/> <see cref="Condition"/>s from all <paramref name="obj"/>'s fields
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>roster of <see cref="OperatorType.Equal"/> <see cref="Condition"/></returns>
    public static Roster<string, Condition> GetConditions(object obj)
    {
        var type = obj.GetType();
        var conditions = new Roster<string, Condition>();
        foreach (var property in type.GetProperties())
        {
            if (NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out _, out _);
            conditions.TryAdd(new(name, property.Name, property.GetValue(obj), OperatorType.Equal));
        }
        return conditions;
    }

    /// <summary>
    /// get many <see cref="OperatorType.Equal"/> <see cref="Condition"/>s from <paramref name="obj"/>'s fields of given <paramref name="propertyNames"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="propertyNames"></param>
    /// <returns>roster of <see cref="OperatorType.Equal"/> <see cref="Condition"/></returns>
    public static Roster<string, Condition> GetConditions(object obj, params string[] propertyNames)
    {
        var type = obj.GetType();
        var conditions = new Roster<string, Condition>();
        foreach (var propertyName in propertyNames)
        {
            var property = type.GetProperty(propertyName);
            if (property is null || NotField(property))
                continue;
            GetFieldNameInfo(property, out var name, out _, out _);
            conditions.TryAdd(new(name, property.Name, property.GetValue(obj), OperatorType.Equal));
        }
        return conditions;
    }
}
