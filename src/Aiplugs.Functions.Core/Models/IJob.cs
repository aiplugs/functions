using System;

namespace Aiplugs.Functions.Core
{
    public interface IJob
    {
        long Id { get; set; }
        string Name { get; set; }
        string Log { get; set; }
        int Progress { get; set; }
        JobStatus Status { get; set; }
        DateTime? StartAt { get; set; }
        DateTime? FinishAt { get; set; }
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        string Parameters { get; set; }
    }
}