using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;

namespace Aiplugs.Functions.Core
{
    public interface IJobRepository
    {
        Task<IJob> FindAsync(long id);
        Task<IEnumerable<IJob>> GetAsync(string name, bool desc = true, long? skipToken = null, int limit = 10);
        Task<long> AddAsync(IJob job);
        Task UpdateAsync(IJob job);
    }
}