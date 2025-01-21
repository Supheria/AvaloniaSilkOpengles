using Microsoft.Data.Sqlite;

namespace LocalUtilities.SQLiteHelper.Converts;

internal sealed class CharConvert : TypeConvert
{
    public override Keywords GetTypeKeyword()
    {
        return Keywords.Integer;
    }

    public override Func<SqliteDataReader, int, object> GetConvert()
    {
        return (r, i) => r.GetChar(i);
    }
}
