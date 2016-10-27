using Sharpitecture.Utils.Logging;
using System;

namespace Sharpitecture.Tasks
{
    public sealed class Task
    {
        /// <summary>
        /// Whether the task is recurring
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// The name of the task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The timeout of the task between each execution
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// The handler of the task
        /// </summary>
        public Action Handler { get; set; }
       
        /// <summary>
        /// The last time this task was executed
        /// </summary>
        public DateTime LastExecution { get; set; }

        /// <summary>
        /// Whether the task can execute again
        /// </summary>
        public bool HasTimedOut { get { return (DateTime.UtcNow - LastExecution).TotalMilliseconds > Timeout; } }

        public Task(Action handler)
        {
            Handler = handler;
        }

        /// <summary>
        /// Executes the task handler
        /// </summary>
        public void Execute()
        {
            if (Handler == null)
            {
                IsRecurring = false;
                Logger.LogF("Task '{0}' had no handler. Removing task from the scheduler", LogType.Warning, Name);
                return;
            }

            Handler.Invoke();
            LastExecution = DateTime.UtcNow;
        }
    }
}
