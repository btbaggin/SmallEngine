using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public abstract class Resource
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

        /// <summary>
        /// Number of references to this resource
        /// </summary>
        internal int RefCount { get; set; }

        /// <summary>
        /// Whether to keep a refernce to this resource after it has been disposed
        /// </summary>
        internal bool KeepReference { get; set; }
        #endregion

        private bool _disposed;

        /// <summary>
        /// Increment the reference count and return the resource.
        /// </summary>
        /// <returns></returns>
        internal Resource Request()
        {
            RefCount += 1;
            if (_disposed)
            {
                Create();
                _disposed = false;
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

        /// <summary>
        /// Decrement the reference count and if nothing is referencing it, dispose of the resource.
        /// </summary>
        internal void DisposeResource(bool pForce)
        {
            RefCount -= 1;
            if ((RefCount <= 0 && !KeepReference) || pForce)
            {
                Dispose();
                _disposed = true;
            }
        }

        public abstract void Dispose();
    }
}
