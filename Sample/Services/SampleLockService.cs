using System;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;

namespace Sample.Services
{
    public class SampleLockService : ILockService
    {
        public Task<bool> LockAsync(string name)
        {
            return Task.FromResult(true);
        }

        public Task UnlockAsync(string name)
        {
            return Task.FromResult(0);
        }
    }
}