using System;
using System.Collections.Generic;
using System.Threading;

namespace Aiplugs.Functions
{
    public interface IContext<TParams>
    {
        ILogger Logger { get; }
        IList<Error> Errors { get; }
        CancellationToken CancellationToken { get; }
        IProgress<int> Progress { get; }
        TParams Parameters { get; }
    }
}