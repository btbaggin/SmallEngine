using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Messages;

namespace Evolusim
{
    class EnemySpawnerComponent : Component
    {
        const int _spawnCount = 3;
        private Timer _spawnTimer = new Timer(60);

        public override void Update(float pDeltaTime)
        {
            if (_spawnTimer.Tick())
            {
                for (int i = 0; i < _spawnCount; i++)
                {
                    Enemy.Create(GameObject.Position);
                }
                Game.Messages.SendMessage(new GameMessage("EnemySpawn", GameObject.Position));
            }
        }
    }
}
