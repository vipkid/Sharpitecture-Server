using Sharpitecture.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sharpitecture.Tasks
{
    public class Scheduler
    {
        private Thread _mainThread;
        private List<Task> _tasks;
        private bool _stopping = false;
        private string _name;

        public string Name { get { return _name; } }

        public Scheduler(string name)
        {
            _tasks = new List<Task>();
            _name = name;
            _mainThread = new Thread(MainLoop);
        }

        public void Start()
        {
            _mainThread.Start();
        }

        public void Stop()
        {
            _stopping = true;
            _mainThread.Join();
            Logger.LogF("Scheduler '{0}' stopped.", LogType.Info, _name);
        }

        public void Enqueue(Task task)
        {
            lock(_tasks)
                _tasks.Add(task);
        }

        public void Enqueue(Action action)
        {
            Task task = new Task(action)
            {
                Name = "Anonymous_Task",
                Handler = action,
                IsRecurring = false,
                Timeout = 0
            };

            lock (_tasks)
                _tasks.Add(task);
        }

        void MainLoop()
        {
            while (!_stopping)
            {
                if (_tasks.Count <= 0) { Thread.Sleep(10); continue; }

                lock (_tasks)
                {
                    Task task;
                    List<Task> toRemove = new List<Task>();

                    for (int i = 0; i < _tasks.Count; i++)
                    {
                        task = _tasks[i];

                        if (!task.IsRecurring)
                            toRemove.Add(task);

                        if (task.HasTimedOut)
                            task.Execute();
                    }

                    foreach (Task _task in toRemove)
                        _tasks.Remove(_task);
                }

                Thread.Sleep(1);
            }

            Logger.LogF("Stop requested on '{0}', aborting thread...", LogType.Info, _name);
        }
    }
}
