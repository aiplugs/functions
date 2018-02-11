using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.Functions.Core
{
    public interface IJobRegistory
    {
        IJob FindJob(long id);
        void AddJob(long id, IJob job);
        void RemoveJob(long id);
        bool ExistJob(long id);
        

        Action FindCanceller(long id);
        void AddCanceller(long id, Action canceller);
        void RemoveCanceller(long id);
        bool ExistCanceller(long id);
        IEnumerable<long> ALlCancellers { get; }

        void Enqueue(long id);
        Task<IJob> DequeueAsync();
    }
}