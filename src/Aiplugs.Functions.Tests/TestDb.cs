using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Aiplugs.Functions.Tests
{
    public class TestDb : IDisposable
    {
        public ITestDb[] DBs { get; }

        public TestDb(bool migration = true)
        {
            var dbs = new List<ITestDb>{ new SQLiteTestDb(migration) };
            var sqlsvr = Environment.GetEnvironmentVariable("AIPLUGS_FUNCTIONS_TESTS_TESTDB");
            if (string.IsNullOrEmpty(sqlsvr) == false)
                dbs.Add(new SqlServerTestDb(sqlsvr, migration));

            DBs = dbs.ToArray();
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
    public class SqlServerTestDb : ITestDb
    {
        public IDbConnection Instance { get; }

        public IJobRepository JobRepository { get { return new Core.Data.SqlServer.JobRepository(Instance); } }

        public SqlServerTestDb(string connectionString, bool migration = true)
        {
            Instance = new SqlConnection(connectionString);

            Instance.Open();
            
            if (migration) 
            {
                Cleanup();
                new Core.Data.SqlServer.Migration(Instance, false).Migrate();
            }
        }

        public void Dispose()
        {
            Instance.Close();
            Instance.Dispose();
        }

        public void Cleanup()
        {
            Instance.Execute(@"DROP TABLE IF EXISTS Jobs;");
        }
    }
}