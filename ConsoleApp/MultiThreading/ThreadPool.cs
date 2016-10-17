using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class ThreadPool<I, O>
    {
        public delegate O HandleTaskDelegate(I input);
        private ConcurrentQueue<I> taskQueue = new ConcurrentQueue<I>();
        private ConcurrentQueue<Thread> freeThreadQueue;
        private List<Thread> busyThreadList = new List<Thread>();
        public ConcurrentBag<O> ResultList { get; private set; }
        public int MaxThreadNumber { get; private set; }
        private HandleTaskDelegate HandleTask;

        public ThreadPool(int maxThreadNumber, HandleTaskDelegate handleTaskDelegate)
        {
            MaxThreadNumber = maxThreadNumber;
            HandleTask = handleTaskDelegate;
            freeThreadQueue = new ConcurrentQueue<Thread>();
            ResultList = new ConcurrentBag<O>();
            InitThreads();
        }

        private void InitThreads()
        {
            Thread thread;
            for(int i = 0; i < MaxThreadNumber; i++)
            {
                thread = new Thread(new ParameterizedThreadStart(Do));
                thread.IsBackground = true;
                freeThreadQueue.Enqueue(thread);
            }
            Thread checkerThread = new Thread(new ThreadStart(ReturnEndedThreads));
            checkerThread.IsBackground = true;
            checkerThread.Start();
        }

        public void AddTask(I task)
        {
            if (task != null)
            {
                taskQueue.Enqueue(task);
                NotifyTaskIsEndedOrNewTask();
            }
            else
            {
                throw new Exception("Task can't be null");
            }
        }

        private void Do(object obj)
        {
            ResultList.Add(HandleTask((I)obj));
        }

        public void Wait()
        {
            while(IsItMade() != true) { }
        }

        private void NotifyTaskIsEndedOrNewTask()
        {
            Thread thread;
            I task;
            while ((freeThreadQueue.Count != 0) && (taskQueue.Count != 0))
            {
                while (!taskQueue.TryDequeue(out task)) { }
                while (!freeThreadQueue.TryDequeue(out thread)) { }
                busyThreadList.Add(thread);
                thread.Start(task);
            }
        }

        private void ReturnEndedThreads()
        {
            while (true)
            {
                if (busyThreadList.Count != 0)
                {
                    for (int i = 0; i < busyThreadList.Count; i++)
                    {
                        if (!busyThreadList[i].IsAlive)
                        {
                            freeThreadQueue.Enqueue(busyThreadList[i]);
                            busyThreadList.RemoveAt(i);
                            NotifyTaskIsEndedOrNewTask();
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        public bool IsItMade()
        {
            return (freeThreadQueue.Count == MaxThreadNumber)
                && (taskQueue.Count == 0);
        }
    }
}
