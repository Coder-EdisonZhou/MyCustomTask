using System;
using System.Threading;
using YZ.Infra.Core;

namespace YZ.Infra.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Demo1_JobRun();
            //Demo2_DedicatedThreadJobScheduler();
            //Demo3_ContinueWith();
            //Demo4_GetAwaiter();

            Console.ReadKey();
        }

        /// <summary>
        /// 01.使用默认TaskScheduler调度器：基于线程池调度
        /// </summary>
        private static void Demo1_JobRun()
        {
            _ = Job.Run(() => Console.WriteLine($"Job1 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job2 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job3 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
        }

        /// <summary>
        /// 02.使用自定义TaskScheduler调度器：使用指定线程调度
        /// </summary>
        private static void Demo2_DedicatedThreadJobScheduler()
        {
            JobScheduler.Current = new DedicatedThreadJobScheduler(2);
            _ = Job.Run(() => Console.WriteLine($"Job1 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job2 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job3 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job4 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job5 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
            _ = Job.Run(() => Console.WriteLine($"Job6 is excuted in thread {Thread.CurrentThread.ManagedThreadId}"));
        }

        /// <summary>
        /// 03.模拟Task.ContinueWith进行异步等待
        /// 要点：多个Job则可以按照预先编排的顺序构成一个链表
        /// </summary>
        private static void Demo3_ContinueWith()
        {
            Job.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("Foo1");
            }).ContinueWith(_ =>
            {
                Thread.Sleep(100);
                Console.WriteLine("Bar1");
            }).ContinueWith(_ =>
            {
                Thread.Sleep(100);
                Console.WriteLine("Baz1");
            });

            Job.Run(() =>
            {
                Thread.Sleep(100);
                Console.WriteLine("Foo2");
            }).ContinueWith(_ =>
            {
                Thread.Sleep(10);
                Console.WriteLine("Bar2");
            }).ContinueWith(_ =>
            {
                Thread.Sleep(10);
                Console.WriteLine("Baz2");
            });
        }

        /// <summary>
        /// 04.实现await关键字的使用
        /// 要点：实现了ICriticalNotifyCompletion接口的JobAwaiter结构体
        /// </summary>
        private static async void Demo4_GetAwaiter()
        {
            await Foo();
            await Bar();
            await Baz();
        }

        static Job Foo() => new Job(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("Foo");
        });

        static Job Bar() => new Job(() =>
        {
            Thread.Sleep(100);
            Console.WriteLine("Bar");
        });

        static Job Baz() => new Job(() =>
        {
            Thread.Sleep(10);
            Console.WriteLine("Baz");
        });
    }
}
