namespace YZ.Infra.Core
{
    public abstract class JobScheduler
    {
        public abstract void QueueJob(Job job);
        public static JobScheduler Current { get; set; } = new ThreadPoolJobScheduler();
    }
}
