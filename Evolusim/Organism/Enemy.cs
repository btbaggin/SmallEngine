using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Enemy : GameObject
    {
        public int AttackPower { get; private set; }

        EnemyMovementComponent _movement;

        static Enemy()
        {
            Scene.Define("enemy", typeof(AnimationRenderComponent),
                                         typeof(EnemyMovementComponent));
        }

        public static Enemy Create(Vector2 pPosition)
        {
            var go = Scene.Current.CreateGameObject<Enemy>("enemy");
            go.Position = pPosition;
            return go;
        }

        public Enemy()
        {
            Scale = new Vector2(Terrain.TerrainMap.BitmapSize);
            AttackPower = 10;
        }

        public override void Initialize()
        {
            var render = GetComponent<AnimationRenderComponent>();
            render.SetBitmap("enemy");
            render.SetAnimation(4, new Vector2(16, 32), .5f, AnimationEval);

            _movement = GetComponent<EnemyMovementComponent>();
        }

        public void Attack(Organism pGameObject)
        {
            pGameObject.Health -= AttackPower;
        }

        private void AnimationEval(AnimationRenderComponent pComponent)
        {
            if (Math.Abs(_movement.Speed.X) - Math.Abs(_movement.Speed.Y) > 0)
            {
                pComponent.AnimationNum = _movement.Speed.X > 0 ? 3 : 1;
            }
            else
            {
                pComponent.AnimationNum = _movement.Speed.Y > 0 ? 2 : 0;
            }
        }
    }
}
