using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSLogger = Microsoft.Extensions.Logging.ILogger;

namespace Aiplugs.Functions.Core
{
    public class JobRunner<TParams> : IDisposable
    {
        private readonly IContextFactory<TParams> _contextFactory;
        private readonly IProcedureResolver _procedureResolver;
        private readonly IJobService _jobService;
        private readonly MSLogger _logger;
        private const string ERR_EXCEPTION_THROWN = "ExceptionThrown";
        CancellationTokenSource CancellationTokenSource;

        public JobRunner(IJobService jobService, IProcedureResolver procedureResolver, IContextFactory<TParams> contextFactory, MSLogger logger)
        {
            if (jobService == null)
                throw new ArgumentNullException(nameof(jobService));

            if (procedureResolver == null)
                throw new ArgumentNullException(nameof(procedureResolver));
            
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));
            
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            
            _jobService = jobService;
            _procedureResolver = procedureResolver;
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public Task Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            return Task.Factory.StartNew(() => {
                while (true)
                {   
                    try
                    {
                        var dequeue = _jobService.DequeueAsync();
                        dequeue.Wait(CancellationTokenSource.Token);
                        var job = dequeue.Result as Job;
                        if (job != null)
                        {
                            Run(job);
                        }
                    }
                    catch(OperationCanceledException)
                    {
                        _logger.LogInformation("JobRunner is notified cancel event.");
                        break;            
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "An error has occured in job");
                    }
                }
            }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        public void Run(Job job)
        {
            var log = new Logger(_logger, job);
            var cts = new CancellationTokenSource();
            var context = _contextFactory.Create(job.GetParameters<TParams>(), log, cts.Token, p => job.Progress = p);

            _jobService.RegisterCanceller(job.Id, () => cts.Cancel(true));
            
            Task.Factory.StartNew(() => 
                {
                    job.StartAt = DateTime.UtcNow;
                    job.Status = JobStatus.Running;
                    try
                    {
                        var method = _procedureResolver.Resolve(job.Name).CreateMethod();
                        method.Invoke(null, new []{ context });
                    }
                    catch(Exception ex)
                    {
                        var baseEx = ex.GetBaseException();
                        if (baseEx.GetType() == typeof(OperationCanceledException))
                        {
                            log.LogInfo("<< The job has been canceled. >>");
                            ExceptionDispatchInfo.Capture(baseEx).Throw();
                        }
                        else
                        {
                            log.LogFail(ex, ex.Message);
                            context.Errors.Add(new Error(ERR_EXCEPTION_THROWN));
                        }
                    }
                }, context.CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
            .ContinueWith(t => 
                {
                    foreach(var error in context.Errors)
                    {
                        log.LogError(error.ToString());
                    }
                    log.Flush();                     
                    job.FinishAt = DateTime.UtcNow;

                    if (t.IsCanceled)
                    {
                        job.Status = JobStatus.Canceled;
                    }
                    else
                    {
                        job.Progress = 100;                    
                        job.Status = context.Errors.Count == 0 ? JobStatus.Success : JobStatus.Faild;
                    }
                    _jobService.SaveAsync(job).Wait(CancellationTokenSource.Token);
                    log.Dispose();
                });
        }
        public void NotifyStop()
        {
            _jobService.CancelAll();
            CancellationTokenSource?.Cancel(true);
        }
        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }
    }
}