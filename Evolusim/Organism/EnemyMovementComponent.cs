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
        private Organism _target;
        private Enemy _gameObject;
        private float _speed = 125;

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
            _target = (Organism)SceneManager.Current.GameObjects.Nearest(GameObject, "Organism");
        }
    }
}
