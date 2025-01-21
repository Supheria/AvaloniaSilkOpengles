using Microsoft.Data.Sqlite;

namespace LocalUtilities.SQLiteHelper.Converts;

internal sealed class FloatConvert : TypeConvert
{
    public override Keywords GetTypeKeyword()
    {
        return Keywords.Real;
    }

    public override Func<SqliteDataReader, int, object> GetConvert()
    {
        return (r, i) => r.GetFloat(i);
    }
}
