using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class IndexedStack<T>
    {
        public int Count { get; private set; }

        T[] _items;
        public IndexedStack()
        {
            _items = new T[8];
        }

        public IndexedStack(int pCapacity)
        {
            _items = new T[pCapacity];
        }

        public int Push(T pValue)
        {
            if (Count >= _items.Length)
            {
                Array.Resize(ref _items, _items.Length + 4);
            }
            _items[Count] = pValue;
            return Count++;
        }

        public T Pop()
        {
            return _items[--Count];
        }

        public T Peek()
        {
            return _items[Count - 1];
        }

        public T PeekAt(int pIndex)
        {
            if (pIndex >= Count) throw new ArgumentException("pIndex must be greater than 0 and less than Count");
            return _items[pIndex];
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}
