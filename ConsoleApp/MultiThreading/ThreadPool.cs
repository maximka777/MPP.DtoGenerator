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
        private Queue<I> taskQueue = new Queue<I>();
        private Queue<Thread> freeThreadQueue;
        private List<Thread> busyThreadList = new List<Thread>();
        public List<O> ResultList { get; private set; }
        public int MaxThreadNumber { get; private set; }
        private HandleTaskDelegate HandleTask;

        public ThreadPool(int maxThreadNumber, HandleTaskDelegate handleTaskDelegate)
        {
            MaxThreadNumber = maxThreadNumber;
            HandleTask = handleTaskDelegate;
            freeThreadQueue = new Queue<Thread>(maxThreadNumber);
            ResultList = new List<O>();
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
            //NotifyTaskIsEndedOrNewTask();
        }

        private void NotifyTaskIsEndedOrNewTask()
        {
            ReturnEndedThreads();
            Thread thread;
            I task;
            while ((freeThreadQueue.Count != 0) && (taskQueue.Count != 0))
            {
                task = taskQueue.Dequeue();
                thread = freeThreadQueue.Dequeue();
                thread.Start(task);
            }
        }

        private void ReturnEndedThreads()
        {
            for(int i = 0; i < busyThreadList.Count; i++)
            {
                if (!busyThreadList[i].IsAlive)
                {
                    freeThreadQueue.Enqueue(busyThreadList[i]);
                    busyThreadList.RemoveAt(i);
                }
            }
        }

        public bool IsItMade()
        {
            return (freeThreadQueue.Count == MaxThreadNumber)
                && (taskQueue.Count == 0);
        }
    }
}
