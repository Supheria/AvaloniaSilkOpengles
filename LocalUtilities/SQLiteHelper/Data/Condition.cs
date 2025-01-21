using LocalUtilities.General;

namespace LocalUtilities.SQLiteHelper;

public sealed class Condition : IRosterItem<string>
{
    public string FieldName { get; }

    public string PropertyName { get; }

    public object? Value { get; set; }

    internal Keywords Operate { get; private set; }

    public string Signature => PropertyName;

    public Condition(string fieldName, string propertyName, object? value, OperatorType operate)
    {
        FieldName = fieldName;
        PropertyName = propertyName;
        Value = value;
        Operate = GetOperate(operate);
    }

    public Condition(string fieldName, object? value, OperatorType operate)
        : this(fieldName, fieldName, value, operate) { }

    public Condition(FieldName? field, object? value, OperatorType operate)
        : this(field?.Name ?? "", field?.PropertyName ?? "", value, operate) { }

    private Keywords GetOperate(OperatorType operate)
    {
        return operate switch
        {
            OperatorType.Equal => Keywords.Equal,
            OperatorType.LessThan => Keywords.Less,
            OperatorType.GreaterThan => Keywords.Greater,
            OperatorType.LessThanOrEqualTo => Keywords.LessOrEqual,
            OperatorType.GreaterThanOrEqualTo => Keywords.GreaterOrEqual,
            _ => Keywords.Blank,
        };
    }

    public void SetOperate(OperatorType operate)
    {
        Operate = GetOperate(operate);
    }

    //public Condition(FieldValue? fieldValue, Operators operate)
    //{
    //    Name = fieldValue?.Name ?? "";
    //    Value = fieldValue?.Value;
    //    Operate = GetOperate(operate);
    //}
}
