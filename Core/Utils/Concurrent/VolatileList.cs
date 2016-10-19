using System;
using System.Collections.Generic;
using System.Linq;

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

        public T FirstOrDefault(Func<T, bool> match)
        {
            lock(_items)
                return _items.FirstOrDefault(match);
        }

        public bool Contains(T value, IEqualityComparer<T> comparer = null)
        {
            lock(_items)
            {
                if (comparer == null)
                    return _items.Contains(value);
                return _items.Contains(value, comparer);
            }
        }

        public bool Any(Func<T, bool> p)
        {
            lock(_items)
                return _items.Any(p);
        }
    }
}
