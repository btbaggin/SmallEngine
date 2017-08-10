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
        static Enemy()
        {
            SceneManager.Define("enemy", typeof(BitmapRenderComponent),
                                         typeof(EnemyMovementComponent));
        }

        public static Enemy Create(Vector2 pPosition)
        {
            var go = SceneManager.Current.CreateGameObject<Enemy>("enemy");
            go.Position = pPosition;
            return go;
        }

        public Enemy()
        {
            Scale = new Vector2(Terrain.TerrainMap.BitmapSize);
        }

        public override void Initialize()
        {
            GetComponent<BitmapRenderComponent>().SetBitmap("enemy");
        }

        public void Attack(Organism pGameObject)
        {
            //pGameObject.Health -= 
        }
    }
}
