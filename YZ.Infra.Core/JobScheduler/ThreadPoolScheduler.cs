using System.Threading;

namespace YZ.Infra.Core
{
    public class ThreadPoolScheduler : JobScheduler
    {
        public override void QueueJob(Job job)
        {
            job.Status = JobStatus.Scheduled;
            var executionContext = ExecutionContext.Capture();
            ThreadPool.QueueUserWorkItem(_ => ExecutionContext.Run(executionContext!, _ => job.Invoke(), null));
        }
    }
}
