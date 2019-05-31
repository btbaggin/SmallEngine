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

        /// <summary>
        /// Returns if the RingBuffer has no data stored
        /// </summary>
        public bool IsEmpty
        {
            get { return _head == _tail; }
        }

        /// <summary>
        /// Creates a new RingBuffer that stores the specified number of entries
        /// </summary>
        /// <param name="pCapacity">The amount of entries to store</param>
        public RingBuffer(int pCapacity)
        {
            _head = 0;
            _tail = 0;

            _capacity = pCapacity;
            _data = new T[_capacity];
        }

        /// <summary>
        /// Pushes a value onto the head of the RingBuffer
        /// </summary>
        public void Push(T pObject)
        {
            _data[_tail] = pObject;
            _tail = (_tail + 1) % _capacity;
        }

        /// <summary>
        /// Returns a value from the tail of the RingBuffer
        /// </summary>
        public T Get()
        {
            if (IsEmpty) throw new InvalidOperationException("Unable to Pop with no elements");

            var t = _data[_head];
            _head = (_head + 1) % _capacity;
            return t;
        }

        /// <summary>
        /// Returns the item most recently added to the RingBuffer
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (IsEmpty) throw new InvalidOperationException("Unable to Peek with no elements");

            return _data[_head];
        }
    }
}
