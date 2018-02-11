using System;
using System.Collections.Generic;
using System.Threading;
using Aiplugs.Functions;

namespace Sample.Models
{
    public class SampleContext : IContext
    {
        public ILogger Logger { get; private set; }

        public IList<Error> Errors { get; } = new List<Error>();

        public CancellationToken CancellationToken { get; private set; }

        public IProgress<int> Progress { get; private set; }
        public SampleContext(ILogger logger, CancellationToken cancellationToken, Action<int> onProgress)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            Progress = new Progress<int>(onProgress);
        }
    }
}