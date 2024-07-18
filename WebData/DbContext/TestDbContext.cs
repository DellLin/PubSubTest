using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebData;
public class TestDbContext : DbContext
{
    public DbSet<PubSubTest> PubSubTest { get; set; }

    public string connectionString { get; }

    public TestDbContext()
    {
        connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "34.81.77.3",
            Port = 5432,
            Username = "benson",
            Password = "benson",
            Database = "pubsub-test"
        }.ToString();
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql(connectionString);
}
public class PubSubTest
{
    public int Id { get; set; }
    public string Message { get; set; }
}