using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    class RingBuffer<T>
    {
        private int _head;
        private int _tail;
        private readonly int _capacity;
        private readonly T[] _data;

        public bool IsEmpty
        {
            get { return _head == _tail; }
        }

        public RingBuffer(int pCapacity)
        {
            _head = 0;
            _tail = 0;

            _capacity = pCapacity;
            _data = new T[_capacity];
        }

        public void Push(T pObject)
        {
            _data[_tail] = pObject;
            _tail = (_tail + 1) % _capacity;
        }

        public T Pop()
        {
            if (IsEmpty) throw new InvalidOperationException("Unable to Pop with no elements");

            var t = _data[_head];
            _head = (_head + 1) % _capacity;
            return t;
        }

        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Unable to Peek with no elements");

            return _data[_head];
        }
    }
}
