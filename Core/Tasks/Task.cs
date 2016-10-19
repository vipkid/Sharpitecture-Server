using Sharpitecture.Utils.Logging;
using System;

namespace Sharpitecture.Tasks
{
    public sealed class Task
    {
        public bool IsRecurring { get; set; }
        public string Name { get; set; }
        public int Timeout { get; set; }
        public Action Handler { get; set; }
        private DateTime _lastExecute = DateTime.MinValue;

        public bool HasTimedOut { get { return (DateTime.UtcNow - _lastExecute).TotalMilliseconds > Timeout; } }

        public Task(Action handler)
        {
            Handler = handler;
        }

        public void Execute()
        {
            if (Handler == null)
            {
                IsRecurring = false;
                Logger.LogF("Task '{0}' had no handler. Removing task from the scheduler", LogType.Warning, Name);
                return;
            }

            Handler.Invoke();
            _lastExecute = DateTime.UtcNow;
        }
    }
}
