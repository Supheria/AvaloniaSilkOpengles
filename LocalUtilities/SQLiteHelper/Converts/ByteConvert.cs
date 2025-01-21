using Microsoft.Data.Sqlite;

namespace LocalUtilities.SQLiteHelper.Converts;

internal sealed class ByteConvert : TypeConvert
{
    public override Keywords GetTypeKeyword()
    {
        return Keywords.Integer;
    }

    public override Func<SqliteDataReader, int, object> GetConvert()
    {
        return (r, i) => r.GetByte(i);
    }
}
