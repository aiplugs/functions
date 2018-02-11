using System;
using System.Threading;
using System.Threading.Tasks;
using Aiplugs.Functions;
using Aiplugs.Functions.Core;
using Sample.Models;

namespace Sample.Services
{
    public class SampleContextFactory : IContextFactory
    {
        public IContext Create(ILogger logger, CancellationToken token, Action<int> onProgress)
        {
            return new SampleContext(logger, token, onProgress);
        }
    }
}