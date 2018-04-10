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
            public void Migrate(IDbTransaction tran)
            {
                _db.Execute(@"CREATE TABLE
                            Jobs (
                                Id     BIGINT IDENTITY,
                                Name   NVARCHAR(255) NOT NULL,
                                Parameters NVARCHAR(MAX) NULL,
                                Progress   INT NOT NULL,
                                Status     INT NOT NULL,
                                StartAt    DATETIME2 NULL,
                                FinishAt   DATETIME2 NULL,
                                Log        NVARCHAR(MAX) NULL,
                                CreatedAt  DATETIME2 NOT NULL,
                                CreatedBy  NVARCHAR(64) NOT NULL
                            )", transaction:tran);
            }

            public bool NeedMigrate(IDbTransaction tran)
            {
                return _db.ExecuteScalar<int>(@"SELECT COUNT(name) FROM sys.tables WHERE name = 'Jobs'", transaction:tran) == 0;
            }
        }
        #endregion
    }
}