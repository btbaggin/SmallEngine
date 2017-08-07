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
        public EnemySpawner(int pX, int pY)
        {
            Position = Terrain.TerrainMap.GetPosition(pX, pY);
            Scale = new Vector2(Terrain.TerrainMap.BitmapSize);

            _spawnTime = 60;
            _spawnCount = 2;
        }

        public override void Update(float pDeltaTime)
        {
            if ((_spawnTimer += pDeltaTime) >= _spawnTime)
            {
                _spawnTimer = 0;
                for(int i = 0; i < _spawnCount; i++)
                {

                }
            }
        }
    }
}
