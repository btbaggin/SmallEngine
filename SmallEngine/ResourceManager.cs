using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public static class ResourceManager
    {
        private static Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
        private static Dictionary<string, string[]> _groups = new Dictionary<string, string[]>();

        #region Public functions
        /// <summary>
        /// Synchronously loads the resource
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pKeepReference">Whether or not to keep a reference if resource has been disposed so it can be reloaded.</param>
        /// <returns>Handle to the resource</returns>
        public static T Add<T>(string pAlias, string pPath) where T : Resource, new()
        {
            System.Diagnostics.Debug.Assert(!_resources.ContainsKey(pAlias));

            //Add to dictionary
            //Set to default resource
            var r = new T()
            {
                Path = System.IO.Path.GetFullPath(pPath),
                Alias = pAlias
            };            
            _resources.Add(pAlias, r);
            //Begin sync load
            r.Create();
            return r;
        }

        public static T Add<T>(string pAlias, T pResource) where T : Resource, new()
        {
            System.Diagnostics.Debug.Assert(!_resources.ContainsKey(pAlias));

            pResource.Alias = pAlias;
            _resources.Add(pAlias, pResource);
            return pResource;
        }

        /// <summary>
        /// Asynchronously loads the resource.
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath) where T : Resource, new()
        {
            return await AddAsync<T>(pAlias, pPath, false, null);
        }

        /// <summary>
        /// Asynchronously loads the resource.
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pKeepReference">Whether or not to keep a reference if resource has been disposed so it can be reloaded.</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath, bool pKeepReference) where T : Resource, new()
        {
            return await AddAsync<T>(pAlias, pPath, pKeepReference, null);
        }

        /// <summary>
        /// Asynchronously loads the resource.
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pCallback">Method to call when loading the resource is complete</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath, Action<T> pCallback) where T : Resource, new()
        {
            return await AddAsync(pAlias, pPath, false, pCallback);
        }

        /// <summary>
        /// Asynchronously loads the resource.
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pCallback">Method to call when loading the resource is complete</param>
        /// <param name="pKeepReference">Whether or not to keep a reference if resource has been disposed so it can be reloaded.</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath, bool pKeepReference, Action<T> pCallback) where T : Resource, new()
        {

            T r;
            System.Diagnostics.Debug.Assert(!_resources.ContainsKey(pAlias));

            //Add to dictionary
            //Set to default resource
            r = new T()
            {
                Path = System.IO.Path.GetFullPath(pPath),
                Alias = pAlias
            };
            _resources.Add(pAlias, r);

            //Begin async load
            await r.CreateAsync();
            if (pCallback != null)
            {
                pCallback.Invoke(r);
            }
            return r;
        }

        public static bool ResourceLoaded(string pAlias)
        {
            return _resources.ContainsKey(pAlias);
        }

        /// <summary>
        /// Used be the deserialization constructor to maintain the current ref count;
        /// </summary>
        /// <param name="pAlias"></param>
        /// <returns></returns>
        internal static int GetRefCount(string pAlias)
        {
            return _resources[pAlias].ReferenceCount;
        }

        /// <summary>
        /// Request a resource from the manager
        /// </summary>
        /// <typeparam name="T">Type of the resource to request</typeparam>
        /// <param name="pAlias">Name of the resource to request</param>
        /// <returns>Null if resource is not present; otherwise a handle to the resource</returns>
        public static T Request<T>(string pAlias) where T : Resource, new()
        {
            System.Diagnostics.Debug.Assert(_resources.ContainsKey(pAlias));
            var r = _resources[pAlias];
            return (T)r.Request();
        }

        public static T RequestFromGroup<T>(string pGroup) where T : Resource, new()
        {
            System.Diagnostics.Debug.Assert(_groups.ContainsKey(pGroup));
            var r = new Random();
            return Request<T>(_groups[pGroup][r.Next(0, _groups[pGroup].Length)]);
        }

        /// <summary>
        /// Dispose of a given resource.
        /// </summary>
        /// <typeparam name="T">Type of the resource to dispose</typeparam>
        /// <param name="pAlias">Name of the resource to dispose</param>
        public static void DisposeResource(string pAlias, bool pForce)
        {
            if (_resources.ContainsKey(pAlias))
            {
                var r = _resources[pAlias];
                if (!pForce) r.Dispose();
                else r.ForceDispose();

                //If we don't want to keep a reference to it, remove it from the dictionary so we can't reload it.
                if (pForce) _resources.Remove(pAlias);
            }
        }

        /// <summary>
        /// Reload all resources from their source.
        /// </summary>
        public static void ReloadResources()
        {
            foreach (Resource r in _resources.Values)
            {
                r.CreateAsync();
            }
        }

        /// <summary>
        /// Reload a single resource from it's source
        /// </summary>
        /// <param name="pAlias">Name of the resource to reload</param>
        public static void ReloadResource(string pAlias)
        {
            var r = _resources[pAlias];
            r.CreateAsync();
        }

        public static void AddGroup(string pGroup, params string[] pAlias)
        {
            if(!_groups.ContainsKey(pGroup))
            {
               _groups.Add(pGroup, pAlias);
            }
        }

        /// <summary>
        /// Completely disposes of all resources
        /// </summary>
        public static void DisposeResources()
        {
            foreach (Resource r in _resources.Values)
            {
                r.ForceDispose();
            }
            _resources = null;
        }
        #endregion
    }
}
