using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;

namespace Evolusim.Terrain
{
    class VegetationLifeComponent : DependencyComponent
    {
        public int LifeTime { get; set; }
        public int SpreadCount { get; set; }

        [ImportComponent]
        private BitmapRenderComponent _render = null;
        private Vegetation _gameObject;

        public override void OnAdded(IGameObject pGameObject)
        {
            base.OnAdded(pGameObject);
            _gameObject = (Vegetation)GameObject;
        }

        Timer _t = new Timer(1);
        public override void Update(float pDeltaTime)
        {
            if(_t.Tick())
            {
                LifeTime -= 1;

                if (LifeTime == 2)
                {
                    Spread();
                }
                else if (LifeTime == 1)
                {
                    _render.SetBitmap(_render.Bitmap.Alias + "_dead");
                }
                else if (LifeTime <= 0)
                {
                    GameObject.Destroy();
                }
            }
        }

        private void Spread()
        {
            for (int i = 0; i < SpreadCount; i++)
            {
                var dx = Generator.Random.Next(_gameObject.X - 1, _gameObject.X + 1);
                var dy = Generator.Random.Next(_gameObject.Y - 1, _gameObject.Y + 1);
                dx = (int)MathF.Clamp(dx, 0, TerrainMap.Size);
                dy = (int)MathF.Clamp(dy, 0, TerrainMap.Size);

                if (TerrainMap.GetTerrainType(dx, dy) == _gameObject.Terrain)
                {
                    Vegetation.Create(dx, dy);
                }
            }
        }
    }
}
