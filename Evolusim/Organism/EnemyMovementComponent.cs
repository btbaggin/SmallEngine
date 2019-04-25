using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim
{
    class EnemyMovementComponent : Component
    {
        Organism _target;
        Enemy _gameObject;
        const float _speed = 125;

        public Vector2 Speed { get { return Vector2.UnitX; } }

        public EnemyMovementComponent() : base()
        {
        }

        public override void Update(float pDeltaTime)
        {
            var nextPos = Vector2.MoveTowards(GameObject.Position, _target.Position, _speed * pDeltaTime);
            GameObject.Position = nextPos;

            if (GameObject.Position == _target.Position)
                _gameObject.Attack(_target);
        }

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);
            _gameObject = (Enemy)pGameObject;
            _target = (Organism)Scene.Current.GameObjects.Nearest(GameObject, "Organism");
        }
    }
}
