using System;

namespace Aiplugs.Functions.Core
{
    public interface IJobInfo
    {
        string Name { get; }
        DateTime CreatedAt { get; }
        string CreatedBy { get; }
        TParams GetParameters<TParams>();
    }
}