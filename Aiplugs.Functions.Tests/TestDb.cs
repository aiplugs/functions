using System;
using System.Data;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;
using Microsoft.Data.Sqlite;

namespace Aiplugs.Functions.Tests
{
    public class TestDb : IDisposable
    {
        public ITestDb[] DBs { get; }

        public TestDb(bool migration = true)
        {
            DBs = new []
            {
                new SQLiteTestDb(migration)
            };
        }
        
        public void Dispose()
        {
            foreach(var db in DBs)
            {
                db.Dispose();
            }
        }
    }
    public interface ITestDb : IDisposable
    {
        IDbConnection Instance { get; }
        IJobRepository JobRepository { get; }
    }
    public class SQLiteTestDb : ITestDb
    {
        public IDbConnection Instance { get; }
        public IJobRepository JobRepository { get { return new Core.Data.Sqlite.JobRepository(Instance); } }
        public SQLiteTestDb(bool migration = true)
        {
            Instance = new SqliteConnection("DataSource=:memory:");

            Instance.Open();

            if (migration)
                new Core.Data.Sqlite.Migration(Instance, true).Migrate();
        }
        public void Dispose()
        {
            Instance.Close();
            Instance.Dispose();
        }
    }
}