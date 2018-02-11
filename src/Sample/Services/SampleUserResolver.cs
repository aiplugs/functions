using System;
using Aiplugs.Functions.Core;

namespace Sample.Services
{
    public class SampleUserResolver : IUserResolver
    {
        public string GetUserId()
        {
            return Guid.Empty.ToString();
        }
    }
}