using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.Functions.Core.Data;

namespace Aiplugs.Functions.Core
{

    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IJobRegistory _jobRegistory;
        private readonly ILockService _lockService;
        private readonly IUserResolver _userResolver;

        public JobService(IJobRepository jobRepository, IJobRegistory jobRegistory, ILockService lockService, IUserResolver userResolver)
        {
            if (jobRepository == null)
                throw new ArgumentNullException(nameof(jobRepository));
            
            if (jobRegistory == null)
                throw new ArgumentNullException(nameof(jobRegistory));
            
            if (lockService == null)
                throw new ArgumentNullException(nameof(lockService));
            
            if (userResolver == null)
                throw new ArgumentNullException(nameof(userResolver));

            _jobRepository = jobRepository;
            _jobRegistory = jobRegistory;
            _lockService = lockService;
            _userResolver = userResolver;
        }
        public void RegisterCanceller(long id, Action canceller)
        {
            _jobRegistory.AddCanceller(id, canceller);
        }
        public void UnregisterCanceller(long id)
        {
            _jobRegistory.RemoveCanceller(id);
        }
        public void Cancel(long id)
        {
            var canceller = _jobRegistory.FindCanceller(id);
            if (canceller != null)
            {
                canceller();
                _jobRegistory.RemoveCanceller(id);
            }
        }
        public void CancelAll()
        {
            foreach(var id in _jobRegistory.ALlCancellers)
            {
                _jobRegistory.FindCanceller(id)();
                _jobRegistory.RemoveCanceller(id);
            }
        }
        public async Task<IJob> DequeueAsync()
        {
            return await _jobRegistory.DequeueAsync();
        }

        public async Task<long?> ExclusiveCreateAsync<TParams>(string name, TParams @params)
        {
            var userId = _userResolver.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (await _lockService.LockAsync(name) == false)
                return null;
            
            var job = new Job(name, userId, @params);

            job.Id = await _jobRepository.AddAsync(job);

            _jobRegistory.AddJob(job.Id, job);
            _jobRegistory.Enqueue(job.Id);

            return job.Id;
        }

        public async Task<IEnumerable<IJob>> GetAsync(string name, bool desc = true, long? skipToken = null, int limit = 10)
        {
            return await _jobRepository.GetAsync(name, desc, skipToken, limit);
        }
        public async Task<IJob> FindAsync(long id)
        {
            var job = _jobRegistory.FindJob(id);
            if (job != null)
                return job;

            return await _jobRepository.FindAsync(id);
        }

        public async Task SaveAsync(IJob job)
        {
            await _jobRepository.UpdateAsync(job);
            if (job.FinishAt.HasValue)
            {
                _jobRegistory.RemoveJob(job.Id);
                UnregisterCanceller(job.Id);
                await _lockService.UnlockAsync(job.Name);
            }
        }
    }
}