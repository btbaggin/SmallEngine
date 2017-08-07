using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public class ObjectPool<T>
    {
        private Stack<T> _objects;
        public ObjectPool()
        {
            _objects = new Stack<T>();
        }

        public T Get()
        {
            T retval;
            if(_objects.Count > 0) retval = _objects.Pop();
            else retval = default(T);

            return retval;
        }

        public void Release(T pT)
        {
            _objects.Push(pT);
        }
    }
}
