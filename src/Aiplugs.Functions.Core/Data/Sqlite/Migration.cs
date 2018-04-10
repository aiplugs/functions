using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Aiplugs.Functions.Core.Data.Sqlite
{
    public class Migration : MigrationBase
    {
        public Migration(IDbConnection dbConnection, bool force = false) 
            : base(dbConnection, force)
        {
            Migrations = new IMigration[] 
            {
                new InitialMigration(dbConnection),
            };
        }
        #region Migrations
        internal class InitialMigration : IMigration
        {
            private readonly IDbConnection _db;
            public InitialMigration(IDbConnection dbConnection) 
            {
                _db = dbConnection;
            }
            public void Migrate(IDbTransaction tran)
            {
                _db.Execute(@"CREATE TABLE IF NOT EXISTS 
                            Jobs (
                                Id     INTEGER PRIMARY KEY,
                                Name   VARCHAR(255) NOT NULL,
                                Parameters TEXT NULL,
                                Progress   INTEGER NOT NULL,
                                Status     INTEGER NOT NULL,
                                StartAt    DATETIME NULL,
                                FinishAt   DATETIME NULL,
                                Log        TEXT NULL,
                                CreatedAt  DATETIME NOT NULL,
                                CreatedBy  VARCHAR(64) NOT NULL
                            )", tran);
            }

            public bool NeedMigrate(IDbTransaction tran)
            {
                return true;
            }
        }
        #endregion
    }
}