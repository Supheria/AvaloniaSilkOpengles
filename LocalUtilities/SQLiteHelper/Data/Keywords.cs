namespace LocalUtilities.SQLiteHelper;

internal sealed class Keywords
{
    string Value { get; }

    private Keywords(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static Keywords Blank { get; } = new(" ");
    public static Keywords Any { get; } = new("*");
    public static Keywords Equal { get; } = new("=");
    public static Keywords Less { get; } = new("<");
    public static Keywords Greater { get; } = new(">");
    public static Keywords LessOrEqual { get; } = new("<=");
    public static Keywords GreaterOrEqual { get; } = new(">=");
    public static Keywords Quote { get; } = new("'");
    public static Keywords DoubleQuote { get; } = new("\"");
    public static Keywords Finish { get; } = new(";");
    public static Keywords Open { get; } = new("(");
    public static Keywords Close { get; } = new(")");
    public static Keywords Comma { get; } = new(",");
    public static Keywords DataSource { get; } = new("data source");
    public static Keywords Version { get; } = new("version");
    public static Keywords Select { get; } = new("select");
    public static Keywords From { get; } = new("from");
    public static Keywords InsertOrIgnoreInto { get; } = new("insert or ignore into");
    public static Keywords InsertOrReplaceInto { get; } = new("insert or replace into");
    public static Keywords Values { get; } = new("values");
    public static Keywords Update { get; } = new("update");
    public static Keywords Set { get; } = new("set");
    public static Keywords Where { get; } = new("where");
    public static Keywords Delete { get; } = new("Delete From");
    public static Keywords Or { get; } = new("or");
    public static Keywords And { get; } = new("and");
    public static Keywords CreateTableIfNotExists { get; } = new("create table if not exists");
    public static Keywords WithoutRowid { get; } = new("without rowid");
    public static Keywords Text { get; } = new("text");
    public static Keywords PrimaryKeyNotNull { get; } = new("primary key not null");
    public static Keywords Integer { get; } = new("integer");
    public static Keywords Real { get; } = new("real");
    public static Keywords SelectCount { get; } = new("select count");
    public static Keywords Unique { get; } = new("unique");
}
