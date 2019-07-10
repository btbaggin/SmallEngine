using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SmallEngine;

namespace Tests
{
    class SceneTests
    {
        Scene _scene;
        [OneTimeSetUp]
        public void Setup()
        {
            Game.Messages = new SmallEngine.Messages.QueueingMessageBus(1);
            _scene = Scene.Load<Scene>(SceneLoadModes.Additive);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _scene.Unload();
        }

        [Test]
        public void LoadingAdditive()
        {
            var s = Scene.Load<Scene>(SceneLoadModes.Additive);
            Assert.IsTrue(_scene.Active);

            Assert.AreEqual(2, Scene.LoadedSceneCount, "Expected 2 scenes to be loaded");
            s.Unload();
        }

        [Test]
        public void LoadingLoadOver()
        {
            var additionalLoad = Scene.Load<Scene>(SceneLoadModes.Additive);
            var s = Scene.Load<Scene>(SceneLoadModes.LoadOver);
            Assert.IsFalse(_scene.Active);
            Assert.IsFalse(additionalLoad.Active);

            Assert.AreEqual(3, Scene.LoadedSceneCount, "Expected 3 scenes to be loaded");
            s.Unload();
            additionalLoad.Unload();
        }

        [Test]
        public void GameObjects()
        {
            _scene.CreateGameObject<GameObject>();
            _scene.CreateGameObject<GameObject>();
            _scene.CreateGameObject<GameObject>();
            Assert.AreEqual(3, _scene.GetGameObjects().Count, "Intiial object count wrong");

            var s = Scene.Load<Scene>(SceneLoadModes.LoadOver);
            Assert.AreEqual(0, s.GetGameObjects().Count, "LoadOver scene count wrong");
            s.Unload();
        }

        [Test]
        public void NamedObject()
        {
            const string name = "NamedObject";
            _scene.CreateGameObject<GameObject>(name, new SmallEngine.Components.IComponent[] { });
            Assert.DoesNotThrow(() => _scene.FindGameObject<GameObject>(name));
        }

        [Test]
        public void ObjectPointers()
        {
            var go = _scene.CreateGameObject<GameObject>();
            var pointer = go.GetPointer();
            Assert.IsTrue(_scene.TryGetGameObject(pointer, out IGameObject o));
        }

        [Test]
        public void NamedObjectWithDifferentScene()
        {
            const string name = "NamedObjectWithDifferentScene";
            var s = Scene.Load<Scene>(SceneLoadModes.LoadOver);
            s.CreateGameObject<GameObject>(name);

            Assert.Throws<GameObjectNotFoundException>(() => _scene.FindGameObject<IGameObject>(name));
            s.Unload();
        }

        [Test]
        public void ObjectPointersWithDifferentScene()
        {
            var s = Scene.Load<Scene>(SceneLoadModes.LoadOver);
            var go = s.CreateGameObject<GameObject>();
            var pointer = go.GetPointer();
            Assert.IsTrue(_scene.TryGetGameObject(pointer, out IGameObject o));
        }
    }
}
