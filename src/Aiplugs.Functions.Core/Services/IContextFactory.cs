using System;
using System.Threading;

namespace Aiplugs.Functions.Core
{
    public interface IContextFactory<TParams>
    {
        IContext<TParams> Create(IJobInfo job, ILogger logger, CancellationToken token, Action<int> onProgress);
    }
}