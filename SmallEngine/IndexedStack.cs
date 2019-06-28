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

        public ref T Pop()
        {
            return ref _items[--Count];
        }

        public ref T Peek()
        {
            return ref _items[Count - 1];
        }

        public ref T PeekAt(int pIndex)
        {
            if (pIndex >= Count) throw new ArgumentException("pIndex must be greater than 0 and less than Count");
            return ref _items[pIndex];
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}
