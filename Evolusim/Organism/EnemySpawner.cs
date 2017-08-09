using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim
{
    class EnemySpawner : GameObject
    {
        public static EnemySpawner Create(int pX, int pY)
        {
            var es = SceneManager.Current.CreateGameObject<EnemySpawner>(new EnemySpawnerComponent(), 
                                                                         new BitmapRenderComponent("enemy_spawner"));
            es.SetXY(pX, pY);
            return es;
        }

        public EnemySpawner()
        {
            Scale = new Vector2(Terrain.TerrainMap.BitmapSize);
        }

        private void SetXY(int pX, int pY)
        {
            Position = Terrain.TerrainMap.GetPosition(pX, pY);
        }
    }
}
