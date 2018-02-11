using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.Functions.Core
{
    public interface IJobService
    {
        Task<long?> ExclusiveCreateAsync(string name);
        Task<IEnumerable<IJob>> GetAsync(string name, bool desc = true, long? skipToken = null, int limit = 10);
        Task<IJob> FindAsync(long id);
        void Cancel(long id);
        void CancelAll();
        Task<IJob> DequeueAsync();
        Task SaveAsync(IJob job);
        void RegisterCanceller(long id, Action canceller);
        void UnregisterCanceller(long id);
    }
}