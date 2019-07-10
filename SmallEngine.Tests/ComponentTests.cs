using NUnit.Framework;
using SmallEngine;
using SmallEngine.Graphics;
using SmallEngine.Physics;

namespace Tests
{
    public class ComponentTests
    {
        IGameObject _renderGo, _renderGo2;
        Scene _scene;
        [OneTimeSetUp]
        public void Setup()
        {
            Game.Messages = new SmallEngine.Messages.QueueingMessageBus(1);
            Scene.Define("test", typeof(RigidBodyComponent), typeof(ColliderComponent));
            _scene = Scene.Load<Scene>(SceneLoadModes.Additive);

            _renderGo = _scene.CreateGameObject<GameObject>();
            _renderGo.AddComponent(new RenderComponent());

            _renderGo2 = _scene.CreateGameObject<GameObject>();
            _renderGo2.AddComponent(new RenderComponent() { ZIndex = 10 });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _scene.Unload();
        }

        [Test]
        public void ComponentsAdded()
        {
            Assert.AreEqual(2, SmallEngine.Components.Component.GetComponentsOfType(typeof(RenderComponent)).Count);
        }

        [Test]
        public void RenderSorted()
        {
            var componets = SmallEngine.Components.Component.GetComponentsOfType(typeof(RenderComponent));
            Assert.GreaterOrEqual(((RenderComponent)componets[1]).ZIndex, ((RenderComponent)componets[0]).ZIndex);
        }

        [Test]
        public void Depenedencies()
        {
            var depGo = _scene.CreateGameObject<GameObject>("test");
            var depGo2 = _scene.CreateGameObject<GameObject>();
            depGo2.AddComponent(new RigidBodyComponent());
            depGo2.AddComponent(new ColliderComponent());

            var c = (ColliderComponent)depGo.GetComponent(typeof(ColliderComponent));
            Assert.NotNull(c.Body, "Dependencies for template GO failed");

            var c2 = (ColliderComponent)depGo2.GetComponent(typeof(ColliderComponent));
            Assert.NotNull(c2.Body, "Dependencies for non-template GO failed");
        }
    }
}