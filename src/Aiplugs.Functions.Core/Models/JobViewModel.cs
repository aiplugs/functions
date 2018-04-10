using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.Functions.Core
{
    public class JobViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Progress { get; set; }
        public JobStatus Status { get; set; }        
        public DateTime? StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Log {get;set;}
        public JToken Parameters { get; set; }
        public JobViewModel(IJob job)
        {
            Id = job.Id;
            Name = job.Name;
            Progress = job.Progress;
            Status = job.Status;
            StartAt = job.StartAt;
            FinishAt = job.FinishAt;
            CreatedAt = job.CreatedAt;
            CreatedBy = job.CreatedBy;
            Log = job.Log;
            Parameters = JToken.Parse(job.Parameters);
        }
    }
}