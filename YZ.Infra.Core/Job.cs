using System;

namespace YZ.Infra.Core
{
    public class Job
    {
        private readonly Action _work;
        private Job _continue;
        public Job(Action work) => _work = work;
        public JobStatus Status { get; internal set; }

        public void Start(JobScheduler scheduler = null)
        {
            (scheduler ?? JobScheduler.Current).QueueJob(this);
        }

        internal protected virtual void Invoke()
        {
            Status = JobStatus.Running;
            _work();
            Status = JobStatus.Completed;
            _continue?.Start();
        }

        public static Job Run(Action work)
        {
            var job = new Job(work);
            job.Start();
            return job;
        }

        public Job ContinueWith(Action<Job> continuation)
        {
            if (_continue == null)
            {
                var job = new Job(() => continuation(this));
                _continue = job;
            }
            else
            {
                _continue.ContinueWith(continuation);
            }

            return this;
        }

        /// <summary>
        /// 任何一个类型一旦拥有了这样一个GetAwaiter方法，我们就能将await关键词应用在对应的对象上面。
        /// </summary>
        /// <returns>JobAwaiter</returns>
        public JobAwaiter GetAwaiter()
        {
            return new JobAwaiter(this);
        }
    }
}
