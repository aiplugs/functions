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
        public IContext<JObject> Create(JObject @params,ILogger logger, CancellationToken token, Action<int> onProgress)
        {
            return new SampleContext(@params, logger, token, onProgress);
        }
    }
}