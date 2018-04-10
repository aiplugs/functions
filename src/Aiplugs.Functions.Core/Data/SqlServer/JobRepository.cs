using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Aiplugs.Functions.Core.Data.SqlServer
{
    public class JobRepository : JobRepositoryBase
    {
        public JobRepository(IDbConnection dbConnection) 
            : base(dbConnection)
        {
        }
        public override async Task<long> AddAsync(IJob job)
        {
            return await _db.TransactionalAsync(async tran => {
                return await _db.ExecuteScalarAsync<long>(
                        @"INSERT INTO 
                            Jobs (Name, Parameters, Status, Progress, StartAt, FinishAt, Log, CreatedAt, CreatedBy) 
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Parameters, @Status, @Progress, @StartAt, @FinishAt, @Log, @CreatedAt, @CreatedBy)",ToParameters(job), transaction:tran);
            });
        }

        public override async Task<IEnumerable<IJob>> GetAsync(string name, bool desc, long? skipToken, int limit)
        {
            var filter = new List<string>();
            if (string.IsNullOrEmpty(name) == false)
                filter.Add("Name = @Name");
            if (skipToken.HasValue)
                filter.Add($"Id {(desc ? "<" : ">")} @SkipToken");

            var sb = new StringBuilder();
            sb.AppendLine("SELECT TOP (@Limit) Id, Name, Parameters, Status, Progress, StartAt, FinishAt, Log, CreatedAt, CreatedBy");
            sb.AppendLine("FROM Jobs");
            if (filter.Count > 0)
                sb.AppendLine($"WHERE {string.Join(" AND ", filter)}");
            sb.AppendLine($"ORDER BY Id {(desc ? "DESC" : "")}");

            return await _db.NonTransactionalAsync(async () => await _db.QueryAsync<Job>(sb.ToString(), new { Name = name, SkipToken = skipToken, Limit = limit }));
        }
    }
}