using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Aiplugs.Functions.Core.Data.SqlServer
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
            public void Migrate()
            {
                _db.Execute(@"CREATE TABLE
                            Jobs (
                                Id     BIGINT PRIMARY KEY,
                                Name   NVARCHAR(255) NOT NULL,
                                Progress  INT NOT NULL,
                                Status    INT NOT NULL,
                                StartAt   DATETIME NULL,
                                FinishAt  DATETIME NULL,
                                Log       NVARCHAR(MAX) NULL,
                                CreatedAt DATETIME NOT NULL,
                                CreatedBy NVARCHAR(64) NOT NULL
                            )");
            }

            public bool NeedMigrate()
            {
                return _db.ExecuteScalar<int>(@"SELECT COUNT(name) FROM sys.tables WHERE name == 'Jobs'") == 0;
            }
        }
        #endregion
    }
}