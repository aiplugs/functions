using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aiplugs.Functions.Core
{
    public abstract class MigrationBase : IMigration
    {
        private readonly IDbConnection _db;
        private readonly bool _force;
        protected IMigration[] Migrations =  new IMigration[0];
        public MigrationBase(IDbConnection dbConnection, bool force = false) 
        {
            _db = dbConnection;
            _force = force;
        }
        public void Migrate()
        {
            _db.Transactional(() => {
                for(var i = 0; i < Migrations.Length; i++)
                {
                    if (Migrations[i].NeedMigrate() || _force) 
                    {
                        Migrations[i].Migrate();
                    }
                }
            }, IsolationLevel.Serializable);
        }

        public bool NeedMigrate()
        {
            return _db.Transactional(() => {
                return Migrations.Any(m => m.NeedMigrate());
            }, IsolationLevel.Serializable);
        }
    }
}