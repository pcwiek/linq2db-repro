using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Mapping;
using NpgsqlTypes;

namespace linq2db_repro
{
    [Table("test")]
    public class TestEntity
    {

        [NotNull]
        [Column("timestamp_range")]
        public NpgsqlRange<DateTimeOffset> RangeMappedAsDateTimeOffset { get; set; }


        [NotNull]
        [Column("timestamp_range")]
        public NpgsqlRange<DateTime> RangeMappedAsDateTime { get; set; }
    }

    public class Database : DataConnection
    {
        public Database(string connectionString) : base(
            new PostgreSQLDataProvider(PostgreSQLVersion.v95),
            connectionString
        )
        {
        }

        public ITable<TestEntity> TestEntities => GetTable<TestEntity>();
    }

    class Program
    {
        async static Task Main(string[] args)
        {
            const string connectionString = "Host=db;Database=repro;Username=postgres;Password=postgres;";
            await using var db = new Database(connectionString);
            await db.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS test (
                      timestamp_range tstzrange not null 
                    )
                ");
            await db.TestEntities.InsertAsync(() => new TestEntity
            {
                RangeMappedAsDateTimeOffset = new NpgsqlRange<DateTimeOffset>(
                    DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    DateTime.UtcNow)
            });
            var range = await db.TestEntities.Select(a => a.RangeMappedAsDateTimeOffset).FirstAsync();
            Console.WriteLine("Range: {0}", range);
        }
    }
}
