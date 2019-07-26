using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    class SceneGameObjectList
    {
        int _firstNullIndex;
        int _capacity;
        int _startIndex;
        int _endIndex;
        IGameObject[] _gameObjects;
        ushort[] _versions;
        readonly Stack<int> _sceneIndexes;

        public SceneGameObjectList()
        {
            _capacity = 1024;
            _gameObjects = new IGameObject[_capacity];
            _versions = new ushort[_capacity];
            _sceneIndexes = new Stack<int>();
            _sceneIndexes.Push(0);
        }

        public int StartScene(SceneLoadModes pMode)
        {
            switch(pMode)
            {
                case SceneLoadModes.Additive:
                    _sceneIndexes.Push(_startIndex);
                    break;

                case SceneLoadModes.LoadOver:
                    _startIndex = _endIndex;
                    _sceneIndexes.Push(_endIndex);
                    break;

                case SceneLoadModes.Replace:
                    //We need to clear now because the end index gets reset
                    Clear(_startIndex, _endIndex);
                    _endIndex = _startIndex;
                    break;

                default:
                    throw new UnknownEnumException(typeof(SceneLoadModes), pMode);
            }
            return _startIndex;
        }

        public void EndScene()
        {
            _endIndex = _sceneIndexes.Pop();
            _startIndex = _sceneIndexes.Peek();
        }

        public void Add(IGameObject pObject)
        {
            //If we are at the end of the list, move the end index
            pObject.Index = _firstNullIndex;
            pObject.Version = _versions[_firstNullIndex];

            //Grow list if necessary
            if(_endIndex >= _capacity)
            {
                _capacity += (int)(_capacity * .25f);
                Array.Resize(ref _gameObjects, _capacity);
                Array.Resize(ref _versions, _capacity);
            }

            //Put our object in the empty spot and move to the next non-null spot
            _gameObjects[_firstNullIndex] = pObject;
            while (_gameObjects[_firstNullIndex] != null) _firstNullIndex++;
            if (_firstNullIndex > _endIndex) _endIndex = _firstNullIndex;
        }

        public void Remove(IGameObject pObject)
        {
            //Null the object and move our allocation index to this spot
            var i = pObject.Index;
            _gameObjects[i] = null;

            unchecked { _versions[i]++; } //Have the versions wrap back to 0
            if (i < _firstNullIndex) _firstNullIndex = i;
            if (i == _endIndex) _endIndex--;
        }

        public bool GetByPointer(long pPointer, out IGameObject pObject)
        {
            GameObject.GetIndexAndVersion(pPointer, out int i, out ushort version);
            if(_versions[i] == version)
            {
                pObject = _gameObjects[i];
                return true;
            }

            pObject = default;
            return false;
        }

        public void Clear(int pStart, int pEnd)
        {
            if (pEnd == 0) pEnd = _endIndex;
            _firstNullIndex = pStart;
            do
            {
                _gameObjects[pStart] = null;
                unchecked { _versions[pStart]++; } //Have the versions wrap back to 0
            } while (++pStart < pEnd);
        }

        public IList<IGameObject> GetGameObjects(int pStart, int pEnd)
        {
            if (pEnd == 0) pEnd = _endIndex;
            return new ArraySegment<IGameObject>(_gameObjects, pStart, pEnd - pStart);
        }
    }
}