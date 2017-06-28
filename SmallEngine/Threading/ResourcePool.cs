using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Threading
{
    public class Pool<T> where T : new()
    {
        private ConcurrentStack<T> stack;

        public Pool()
        {
            stack = new ConcurrentStack<T>();
        }

        public void Clear()
        {
            stack.Clear();
        }

        public void Give(T obj)
        {
            stack.Push(obj);
        }

        public T Get()
        {
            T freeObj;
            if (!stack.TryPop(out freeObj))
            {
                freeObj = Activator.CreateInstance<T>();
            }

            return freeObj;
        }
    }
}
