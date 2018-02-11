using System;
using System.Threading;

namespace Aiplugs.Functions.Core
{
    public interface IContextFactory
    {
        IContext Create(ILogger logger, CancellationToken token, Action<int> onProgress);
    }
}