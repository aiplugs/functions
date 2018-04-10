using System;
using System.Collections.Generic;
using System.Threading;
using Aiplugs.Functions;
using Newtonsoft.Json.Linq;

namespace Sample.Models
{
    public class SampleContext : IContext
    {
        public ILogger Logger { get; private set; }

        public IList<Error> Errors { get; } = new List<Error>();

        public CancellationToken CancellationToken { get; private set; }

        public IProgress<int> Progress { get; private set; }

        public JObject Parameters { get; private set; }

        public SampleContext(JObject @params, ILogger logger, CancellationToken cancellationToken, Action<int> onProgress)
        {
            Parameters = @params;
            Logger = logger;
            CancellationToken = cancellationToken;
            Progress = new Progress<int>(onProgress);
        }
    }
}