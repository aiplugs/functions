using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aiplugs.Functions.Core
{
    public class Job : IJob, IJobInfo
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
        public string Parameters { get; set; }

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
            : this(name, createBy, null)
        {}
        public Job(string name, string createBy, object @params) 
            : this()
        {
            Name = name;
            CreatedAt = DateTime.UtcNow;            
            CreatedBy = createBy;
            Parameters = JsonConvert.SerializeObject(@params);
        }
        public TParams GetParameters<TParams>()
        {
            return JsonConvert.DeserializeObject<TParams>(Parameters);
        }
    }
}