using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.Functions.Core.Data
{
    public abstract class JobRepositoryBase : IJobRepository
    {
        protected readonly IDbConnection _db;
        public JobRepositoryBase(IDbConnection dbConnection)
        {
            _db = dbConnection;
        }
        
        public abstract Task<long> AddAsync(IJob job);

        public virtual async Task<IJob> FindAsync(long id)
        {
            return await _db.NonTransactionalAsync(async() => await _db.QuerySingleOrDefaultAsync<Job>(
                @"SELECT Id, Name, Progress, Status, StartAt, FinishAt, Log, CreatedAt, CreatedBy 
                    FROM Jobs
                  WHERE Id = @Id", new { Id = id }));
        }

        public abstract Task<IEnumerable<IJob>> GetAsync(string name, bool desc, long? skipToken, int limit);

        public virtual async Task UpdateAsync(IJob job)
        {
            await _db.TransactionalAsync(async tran => {
                return await _db.ExecuteAsync(
                        @"UPDATE Jobs
                            SET Name = @Name,
                                Progress = @Progress,
                                Status = @Status,
                                StartAt = @StartAt,
                                FinishAt = @FinishAt,
                                Log = @Log
                          WHERE Id = @Id", job, transaction:tran);
            });
        }
    }
}