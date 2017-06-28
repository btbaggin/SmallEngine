using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    class ResourceManager
    {
        private static Dictionary<string, Resource> _resources;
        private static Dictionary<Type, string> _directories;

        #region "Constructor"
        static ResourceManager()
        {
            _resources = new Dictionary<string, Resource>();
            _directories = new Dictionary<Type, string>();
        }
        #endregion

        #region "Public functions"
        public static void SetDirectory(Type pType, string pRelativePath)
        {
            _directories.Add(pType, pRelativePath);
        }

        /// <summary>
        /// Synchronously loads the resource
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <returns>Handle to the resource</returns>
        public static T Add<T>(string pAlias, string pPath) where T : Resource, new()
        {
            return Add<T>(pAlias, pPath, false);
        }

        /// <summary>
        /// Synchronously loads the resource
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pKeepReference">Whether or not to keep a refernce if resource has been disposed so it can be reloaded.</param>
        /// <returns>Handle to the resource</returns>
        public static T Add<T>(string pAlias, string pPath, bool pKeepReference) where T : Resource, new()
        {
            if (_resources.ContainsKey(pAlias))
            {
                return Request<T>(pAlias);
            }

            //Add to dictionary
            //Set to default resource
            var r = new T();
            r.Path = _directories.ContainsKey(typeof(T)) ? System.IO.Path.Combine(_directories[typeof(T)], pPath) : pPath;
            r.Alias = pAlias;
            r.KeepReference = pKeepReference;
            _resources.Add(pAlias, r);

            //Begin sync load
            r.Create();
            return (T)r;
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
        /// <param name="pKeepReference">Whether or not to keep a refernce if resource has been disposed so it can be reloaded.</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath, bool pKeepReference) where T : Resource, new()
        {
            return await AddAsync<T>(pAlias, pPath, pKeepReference, null);
        }

        /// <summary>
        /// Asychronously loads the resource.
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pCallback">Method to call when loading the resource is complete</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath, Action<T> pCallback) where T : Resource, new()
        {
            return await AddAsync<T>(pAlias, pPath, false, pCallback);
        }

        /// <summary>
        /// Asychronously loads the resource.
        /// Will return if the resource is already loaded
        /// </summary>
        /// <typeparam name="T">Type of the resource to load</typeparam>
        /// <param name="pAlias">Name to give to the resource</param>
        /// <param name="pPath">Path to the resource</param>
        /// <param name="pCallback">Method to call when loading the resource is complete</param>
        /// <param name="pKeepReference">Whether or not to keep a refernce if resource has been disposed so it can be reloaded.</param>
        /// <returns>Handle to the resource</returns>
        public async static Task<T> AddAsync<T>(string pAlias, string pPath, bool pKeepReference, Action<T> pCallback) where T : Resource, new()
        {

            T r;
            if (_resources.ContainsKey(pAlias))
            {
                r = Request<T>(pAlias);
                if (pCallback != null)
                {
                    pCallback.Invoke(r);
                }
                return r;
            }

            //Add to dictionary
            //Set to default resource
            r = new T();
            r.Path = _directories.ContainsKey(typeof(T)) ? System.IO.Path.Combine(_directories[typeof(T)], pPath) : pPath;
            r.Alias = pAlias;
            r.KeepReference = pKeepReference;
            _resources.Add(pAlias, r);

            //Begin async load
            await r.CreateAsync();
            if (pCallback != null)
            {
                pCallback.Invoke(r);
            }
            return r;
        }

        /// <summary>
        /// Request a resource from the manager
        /// </summary>
        /// <typeparam name="T">Type of the resource to request</typeparam>
        /// <param name="pAlias">Name of the resource to request</param>
        /// <returns>Null if resource is not present; otherwise a handle to the resource</returns>
        public static T Request<T>(string pAlias) where T : Resource, new()
        {
            if (!_resources.ContainsKey(pAlias))
            {
                return null;
            }

            var r = _resources[pAlias];
            return (T)r.Request();
        }

        /// <summary>
        /// Dispose of a given resource.
        /// </summary>
        /// <typeparam name="T">Type of the resource to dispose</typeparam>
        /// <param name="pAlias">Name of the resource to dispose</param>
        public static void Dispose<T>(string pAlias) where T : Resource, new()
        {
            if (_resources.ContainsKey(pAlias))
            {
                var r = _resources[pAlias];
                r.DisposeResource(false);

                //If we don't want to keep a reference to it, remove it from the dictionary so we can't reload it.
                if (!r.KeepReference)
                {
                    _resources.Remove(pAlias);
                }
            }
        }

        /// <summary>
        /// Forces disposal of a resource even if the resource is set to keep a reference
        /// </summary>
        /// <typeparam name="T">Type of the resource to dispose</typeparam>
        /// <param name="pAlias">Name of the resource to dispose</param>
        /// <param name="pForce">Whether or not to force collection of the resource</param>
        public static void ForceDispose<T>(string pAlias) where T : Resource, new()
        {
            if (_resources.ContainsKey(pAlias))
            {
                var r = _resources[pAlias];
                r.DisposeResource(true);

                //Remove refernce to class so GC takes care of it
                _resources.Remove(pAlias);
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

        /// <summary>
        /// Completely disposes of all resources
        /// </summary>
        public static void Dispose()
        {
            foreach (Resource r in _resources.Values)
            {
                r.DisposeResource(true);
            }
            _resources = null;
        }
        #endregion
    }
}
