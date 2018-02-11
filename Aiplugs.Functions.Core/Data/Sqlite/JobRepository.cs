using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.Functions.Core.Data.Sqlite
{
    public class JobRepository : IJobRepository
    {
        private readonly IDbConnection _db;
        public JobRepository(IDbConnection dbConnection)
        {
            _db = dbConnection;
        }
        
        public async Task<long> AddAsync(IJob job)
        {
            return await _db.TransactionalAsync(async () => {
                return await _db.QuerySingleAsync<long>(
                        @"INSERT INTO 
                            Jobs (Name, Status, Progress, StartAt, FinishAt, Log, CreatedAt, CreatedBy) 
                            VALUES (@Name, @Status, @Progress, @StartAt, @FinishAt, @Log, @CreatedAt, @CreatedBy); 
                          SELECT last_insert_rowid();",job);
            });
        }

        public async Task<IJob> FindAsync(long id)
        {
            return await _db.NonTransactionalAsync(async() => await _db.QuerySingleOrDefaultAsync<Job>(
                @"SELECT Id, Name, Progress, Status, StartAt, FinishAt, Log, CreatedAt, CreatedBy 
                    FROM Jobs
                  WHERE Id = @Id", new { Id = id }));
        }

        public async Task<IEnumerable<IJob>> GetAsync(string name, bool desc, long? skipToken, int limit)
        {
            var filter = new List<string>();
            if (string.IsNullOrEmpty(name) == false)
                filter.Add("Name = @Name");
            if (skipToken.HasValue)
                filter.Add($"Id {(desc ? "<" : ">")} @SkipToken");

            var sb = new StringBuilder();
            sb.AppendLine("SELECT Id, Name, Status, Progress, StartAt, FinishAt, Log, CreatedAt, CreatedBy");
            sb.AppendLine("FROM Jobs");
            if (filter.Count > 0)
                sb.AppendLine($"WHERE {string.Join(" AND ", filter)}");
            sb.AppendLine($"ORDER BY Id {(desc ? "DESC" : "")}");
            sb.AppendLine("LIMIT @Limit");

            return await _db.NonTransactionalAsync(async () => await _db.QueryAsync<Job>(sb.ToString(), new { Name = name, SkipToken = skipToken, Limit = limit }));
        }

        public async Task UpdateAsync(IJob job)
        {
            await _db.TransactionalAsync(async () => {
                return await _db.ExecuteAsync(
                        @"UPDATE Jobs
                            SET Name = @Name,
                                Progress = @Progress,
                                Status = @Status,
                                StartAt = @StartAt,
                                FinishAt = @FinishAt,
                                Log = @Log
                          WHERE Id = @Id", job);
            });
        }
    }
}