using Microsoft.Data.Sqlite;

namespace LocalUtilities.SQLiteHelper.Converts;

internal abstract class TypeConvert
{
    public abstract Keywords GetTypeKeyword();
    public abstract Func<SqliteDataReader, int, object> GetConvert();
}
