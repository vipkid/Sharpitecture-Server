using System.Collections.Generic;

namespace Sharpitecture.Utils.Concurrent
{
    public class VolatileQueue<T>
    {
        private Queue<T> _child;
        private object _lockObject = new object();
        
        public bool IsEmpty { get { return _child.Count == 0; } }

        public VolatileQueue()
        {
            _child = new Queue<T>();
        }

        public bool TryDequeue(out T item)
        {
            if (_child.Count <= 0)
            {
                item = default(T);
                return false;
            }

            lock (_lockObject)
            {
                try
                {
                    item = _child.Dequeue();
                    return true;
                }
                catch
                {
                    item = default(T);
                    return false;
                }
            }
        }

        public void Enqueue(T item) => _child.Enqueue(item);
    }
}
