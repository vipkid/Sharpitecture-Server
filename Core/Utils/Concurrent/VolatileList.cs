using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpitecture.Utils.Concurrent
{
    public class VolatileList<T>
    {
        private List<T> _items;
        private object _lock = new object();

        //public List<T> Items { get { return _items; } }

        public VolatileList()
        {
            _items = new List<T>();
        }

        public void Add(T item)
        {
            lock(_lock)
            {
                _items.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock(_lock)
            {
                _items.Remove(item);
            }
        }

        public void ForEach(Action<T> action)
        {
            lock (_items)
            {
                foreach (T item in _items)
                    action(item);
            }
        }

        public List<T> FindAll(Predicate<T> match)
        {
            lock(_items)
                return _items.FindAll(match);
        }
    }
}
