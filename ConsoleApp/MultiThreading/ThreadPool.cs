using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading
{
    public class ThreadPool<I, O> : IDisposable
    {
        private int MAX_TASK_COUNT = 100;

        public delegate O HandleTaskDelegate(I input);
        private ConcurrentQueue<I> taskQueue = new ConcurrentQueue<I>();
        private List<Thread> threadList;
        public ConcurrentBag<O> ResultList { get; private set; }
        public int MaxThreadNumber { get; private set; }
        private HandleTaskDelegate HandleTask;
        Semaphore semaphore;
        private bool work = true;

        public ThreadPool(int maxThreadNumber, HandleTaskDelegate handleTaskDelegate)
        {
            semaphore = new Semaphore(0, MAX_TASK_COUNT);
            MaxThreadNumber = maxThreadNumber;
            HandleTask = handleTaskDelegate;
            threadList = new List<Thread>();
            ResultList = new ConcurrentBag<O>();
            InitThreads();
        }

        private void InitThreads()
        {
            Thread thread;
            for(int i = 0; i < MaxThreadNumber; i++)
            {
                thread = new Thread(new ThreadStart(Do));
                thread.IsBackground = true;
                threadList.Add(thread);
                thread.Start();
            }
        }

        public void AddTask(I task)
        {
            if (task != null)
            {
                taskQueue.Enqueue(task);
                semaphore.Release();
                
            }
            else
            {
                throw new Exception("Task can't be null");
            }
        }

        private void Do()
        {
            I task;
            while (work) {
                semaphore.WaitOne();
                taskQueue.TryDequeue(out task);
                ResultList.Add(HandleTask(task));
            }
        }

        public void Wait()
        {
            while(!IsItMade())
            {
                Thread.Sleep(0);
            }
        }

        private bool AreAllThreadsStoped()
        {
            foreach(Thread thread in threadList)
            {
                if(thread.ThreadState == ThreadState.Running)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsItMade()
        {
            return ((taskQueue.Count == 0) && (AreAllThreadsStoped()));
        }

        public void Dispose()
        {
            work = false;
            Thread.Sleep(10);
            semaphore.Close();
        }
    }
}
