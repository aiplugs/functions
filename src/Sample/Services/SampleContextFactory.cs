using System;
using System.Threading;
using System.Threading.Tasks;
using Aiplugs.Functions;
using Aiplugs.Functions.Core;
using Newtonsoft.Json.Linq;
using Sample.Models;

namespace Sample.Services
{
    public class SampleContextFactory : IContextFactory<JObject>
    {
        public IContext<JObject> Create(IJobInfo job, ILogger logger, CancellationToken token, Action<int> onProgress)
        {
            return new SampleContext(job.GetParameters<JObject>(), logger, token, onProgress);
        }
    }
}