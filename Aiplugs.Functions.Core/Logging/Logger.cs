using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using IMSLogger = Microsoft.Extensions.Logging.ILogger;
using System.Text;

namespace Aiplugs.Functions.Core
{
    public class Logger : ILogger
    {
        private readonly IMSLogger _logger;
        private readonly IJob _job;
        private readonly TextWriter _writer;
        public Logger(IMSLogger logger, IJob job)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (job == null)
                throw new ArgumentNullException(nameof(job));

            _logger = logger;
            _job = job;
            _writer = new StringWriter(new StringBuilder(job.Log));
        }
        public void Log(object message)
        {
            var json = JsonConvert.SerializeObject(message);
            _writer.WriteLine($"[Log] [{DateTimeOffset.Now:o}] {json}");
            _logger.LogInformation(json);
            WriteOut();            
        }
        public void LogFail(Exception ex, string message)
        {
            _writer.WriteLine($"[Fail] [{DateTimeOffset.Now:o}] {message}\n{ex}");
            _logger.LogCritical(ex, message);
            WriteOut();            
        }
        public void LogError(string message)
        {
            _writer.WriteLine($"[Error] [{DateTimeOffset.Now:o}] {message}");
            _logger.LogError(message);
            WriteOut();
        }
        public void LogInfo(string message)
        {
            _writer.WriteLine($"[Info] [{DateTimeOffset.Now:o}] {message}");
            _logger.LogInformation(message);
            WriteOut();
        }

        private void WriteOut()
        {
            _job.Log = _writer.ToString();
        }
        public void Flush()
        {
            _writer.Flush();
            WriteOut();
        }

        public void Dispose()
        {
            Flush();
            _writer?.Dispose();
        }
    }
}