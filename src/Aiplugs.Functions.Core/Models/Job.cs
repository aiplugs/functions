using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aiplugs.Functions.Core
{
    public class Job : IJob
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

        public Job() 
        {
            Log = string.Empty;
            Progress = 0;
            Status = JobStatus.Ready;
            StartAt = null;
            FinishAt = null;
            CreatedBy = Guid.Empty.ToString();
        }
        public Job(string name, string createBy) 
            : this()
        {
            Name = name;
            CreatedAt = DateTime.UtcNow;            
            CreatedBy = createBy;
        }
    }
}