using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;

namespace Aiplugs.Functions.Core
{
    public class JobRegistory : IJobRegistory
    {
        private IDictionary<long, IJob> _jobs = new ConcurrentDictionary<long, IJob>();
        private IDictionary<long, Action> _cancellers = new ConcurrentDictionary<long, Action>();
        private ConcurrentQueue<long> _queue = new ConcurrentQueue<long>();

        public IJob FindJob(long id)
        {
            if (ExistJob(id) == false)
                return null;
            
            return _jobs[id];
        }

        public void AddJob(long id, IJob job)
        {
            if (ExistJob(id) == false)
                _jobs.Add(id, job);
        }

        public void RemoveJob(long id)
        {
            if (ExistJob(id))
                _jobs.Remove(id);
        }

        public bool ExistJob(long id)
        {
            return _jobs.ContainsKey(id);
        }

        public Action FindCanceller(long id)
        {
            if (ExistCanceller(id) == false)
                return null;
            
            return _cancellers[id];
        }

        public void AddCanceller(long id, Action canceller)
        {
            if (ExistCanceller(id) == false)
                _cancellers.Add(id, canceller);
        }

        public void RemoveCanceller(long id)
        {
            if (ExistCanceller(id))
                _cancellers.Remove(id);
        }

        public bool ExistCanceller(long id)
        {
            return _cancellers.ContainsKey(id);
        }

        public IEnumerable<long> ALlCancellers
        {
            get 
            {
                return _cancellers.Keys;
            }
        }

        public void Enqueue(long id)
        {
            _queue.Enqueue(id);
        }

        public async Task<IJob> DequeueAsync()
        {
            while(true)
            {
                if (_queue.IsEmpty)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    continue;
                }
                long id;
                if (_queue.TryDequeue(out id))
                    return FindJob(id);
            }
        }
    }
}