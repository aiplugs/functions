using Aiplugs.Functions.Core;
using Sample.Models;

namespace Sample.Services
{
    public class SampleProcedureResolver : IProcedureResolver
    {
        public IProcedure Resolve(string name)
        {
            return new SampleProcedure(name);
        }
    }
}