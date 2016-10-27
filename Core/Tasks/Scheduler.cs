using Sharpitecture.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sharpitecture.Tasks
{
    public class Scheduler
    {
        /// <summary>
        /// The list of tasks executed by the scheduler
        /// </summary>
        public List<Task> Tasks { get; private set; }

        /// <summary>
        /// Returns whether the scheduler is in the process of stopping
        /// </summary>
        public bool IsStopping { get; private set; }

        /// <summary>
        /// The name of the scheduler
        /// </summary>
        public string Name { get; private set; }

        private Thread _mainThread;

        public Scheduler(string name)
        {
            Tasks = new List<Task>();
            Name = name;
            _mainThread = new Thread(MainLoop);
        }

        /// <summary>
        /// Starts the scheduler
        /// </summary>
        public void Start()
        {
            _mainThread.Start();
        }

        /// <summary>
        /// Stops the execution of tasks
        /// </summary>
        public void Stop()
        {
            IsStopping = true;
            _mainThread.Join();
            Logger.LogF("Scheduler '{0}' stopped.", LogType.Info, Name);
        }

        /// <summary>
        /// Queues a task to the scheduler
        /// </summary>
        public void Enqueue(Task task)
        {
            lock(Tasks)
                Tasks.Add(task);
        }

        /// <summary>
        /// Queues an anonymous task to the scheduler
        /// </summary>
        public void Enqueue(Action action)
        {
            Task task = new Task(action)
            {
                Name = "Anonymous_Task",
                Handler = action,
                IsRecurring = false,
                Timeout = 0
            };

            lock (Tasks)
                Tasks.Add(task);
        }

        void MainLoop()
        {
            while (!IsStopping)
            {
                if (Tasks.Count <= 0) { Thread.Sleep(10); continue; }

                lock (Tasks)
                {
                    Task task;
                    List<Task> toRemove = new List<Task>();

                    for (int i = 0; i < Tasks.Count; i++)
                    {
                        task = Tasks[i];

                        if (!task.IsRecurring)
                            toRemove.Add(task);

                        if (task.HasTimedOut)
                            task.Execute();
                    }

                    foreach (Task _task in toRemove)
                        Tasks.Remove(_task);
                }

                Thread.Sleep(1);
            }

            Logger.LogF("Stop requested on '{0}', aborting thread...", LogType.Info, Name);
        }
    }
}
