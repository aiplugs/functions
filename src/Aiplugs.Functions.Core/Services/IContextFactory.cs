using System;
using System.Threading;

namespace Aiplugs.Functions.Core
{
    public interface IContextFactory<TParams>
    {
        IContext<TParams> Create(TParams @params, ILogger logger, CancellationToken token, Action<int> onProgress);
    }
}