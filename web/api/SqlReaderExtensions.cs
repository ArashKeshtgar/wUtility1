using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

// A single generic mapper, driven by a lambda per call site, replacing the
// repeated "while (await reader.ReadAsync()) { list.Add(new Foo(reader.Get...)) }"
// boilerplate that DoctorsService/DbConfigService/SchemaReader each had.
public static class SqlReaderExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this SqlDataReader reader, Func<SqlDataReader, T> map)
    {
        var results = new List<T>();
        while (await reader.ReadAsync())
            results.Add(map(reader));
        return results;
    }
}
