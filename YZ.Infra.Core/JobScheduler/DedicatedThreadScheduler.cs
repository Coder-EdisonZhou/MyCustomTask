using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace YZ.Infra.Core
{
    internal class DedicatedThreadScheduler : JobScheduler
    {
        private readonly Queue<Job>[] _queues;
        private readonly Thread[] _threads;
        private readonly ManualResetEvent[] _events;

        public DedicatedThreadScheduler(int threadCount)
        {
            _queues = new Queue<Job>[threadCount];
            _threads = new Thread[threadCount];
            _events = new ManualResetEvent[threadCount];

            for (int index = 0; index < threadCount; index++)
            {
                var queue = _queues[index] = new Queue<Job>();
                var thread = _threads[index] = new Thread(Invoke);
                _events[index] = new ManualResetEvent(true);
                thread.Start(index);
            }

            void Invoke(object? state)
            {
                var index = (int)state!;
                var @event = _events[index];
                while (true)
                {
                    if (@event.WaitOne())
                    {
                        while (true)
                        {
                            if (!_queues[index].TryDequeue(out var job))
                            {
                                Suspend(index);
                                break;
                            }
                            job.Invoke();
                        }
                    }
                }
            }
        }

        public override void QueueJob(Job job)
        {
            job.Status = JobStatus.Scheduled;
            var (queue, index) = _queues.Select((queue, index) => (queue, index)).OrderBy(it => it.queue.Count).First();
            queue.Enqueue(job);
            Resume(index);
        }

        public void Suspend(int index) => _events[index].Reset();
        public void Resume(int index) => _events[index].Set();
    }
}
