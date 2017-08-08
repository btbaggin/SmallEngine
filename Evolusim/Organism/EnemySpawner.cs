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
        float _spawnTime;
        float _spawnTimer;
        int _spawnCount;

        public static EnemySpawner Create(int pX, int pY)
        {
            var es = SceneManager.Current.CreateGameObject<EnemySpawner>();
            es.SetXY(pX, pY);
            return es;
        }

        public EnemySpawner()
        {
            Scale = new Vector2(Terrain.TerrainMap.BitmapSize);

            _spawnTime = 60;
            _spawnCount = 2;
        }

        public override void Initialize()
        {
            GetComponent<BitmapRenderComponent>().SetBitmap("enemy_spawner");
        }

        private void SetXY(int pX, int pY)
        {
            Position = Terrain.TerrainMap.GetPosition(pX, pY);
        }

        public override void Update(float pDeltaTime)
        {

        }
    }
}
