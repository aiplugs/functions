using System;
using System.Data;

namespace Aiplugs.Functions.Core
{
    public class OptionsBuilder
    {
        private bool _forceMigration = false;
        public Func<IDbConnection, IMigration> MigrationFactory { get; private set; }
        public Func<IDbConnection, IJobRepository> JobRepositoryFactory { get; private set; }
        public OptionsBuilder()
        {
            UseSqlite();
        }
        public OptionsBuilder UseSqlite()
        {
            MigrationFactory = db => new Data.Sqlite.Migration(db, _forceMigration);
            JobRepositoryFactory = db => new Data.Sqlite.JobRepository(db);
            return this;
        }
        public OptionsBuilder UseSqlServer()
        {
            MigrationFactory = db => new Data.Sqlite.Migration(db, _forceMigration);
            JobRepositoryFactory = db => new Data.Sqlite.JobRepository(db);            
            return this;
        }
        public OptionsBuilder ForceMigration()
        {
            _forceMigration = true;
            return this;
        }
    }
}