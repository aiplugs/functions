using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Sample.Data
{
    /// <summary>
    /// SampleDb singleton for in memory db.
    /// </summary>
    public static class SampleDb
    {
        public static IDbConnection Instance { get; }
        static SampleDb()
        {
            Instance = new SqliteConnection("DataSource=:memory:");
            Instance.Open();
        }

        public static void CloseAndDispose()
        {
            Instance.Close();
            Instance.Dispose();
        }
    }
}