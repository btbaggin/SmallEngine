using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    [Serializable]
    public abstract class Resource : IDisposable, ISerializable
    {
        #region Public properties
        /// <summary>
        /// Path to the resource for loading.
        /// </summary>
        internal string Path { get; set; }

        /// <summary>
        /// Name to refer to the resource
        /// </summary>
        public string Alias { get; internal set; }

        public bool Disposed { get; private set; }
        #endregion

        protected int _refCount;

        /// <summary>
        /// Increment the reference count and return the resource.
        /// </summary>
        /// <returns></returns>
        internal Resource Request()
        {
            _refCount += 1;
            if (Disposed)
            {
                Create();
                Disposed = false;
            }
            return this;
        }

        /// <summary>
        /// Asynchronously load the resource requested.
        /// This MUST mutate this instead of replacing it
        /// </summary>
        /// <param name="pPath">Path to the file to load</param>
        /// <returns></returns>
        internal abstract Task CreateAsync();

        /// <summary>
        /// Synchronously load the resource requested.
        /// This MUST mutate this instead of replacing it
        /// </summary>
        /// <param name="pPath">Path to the file to load</param>
        /// <returns></returns>
        internal abstract void Create();

        protected Resource() { }

        protected Resource(SerializationInfo pInfo, StreamingContext pContext)
        {
            Path = pInfo.GetString("Path");
            Alias = pInfo.GetString("Alias");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Path", Path);
            info.AddValue("Alias", Alias);
        }

        /// <summary>
        /// Decrement the reference count and if nothing is referencing it, dispose of the resource.
        /// </summary>
        protected abstract void DisposeResource();

        internal void ForceDispose()
        {
            _refCount = 0;
            DisposeResource();
            Disposed = true;
        }

        public void Dispose()
        {
            _refCount -= 1;
            if (_refCount <= 0)
            {
                DisposeResource();
                Disposed = true;
            }
        }
    }
}
