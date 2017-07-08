using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallEngine;
using SmallEngine.Input;
using SmallEngine.Graphics;

namespace Evolusim
{
    class Evolusim : Game
    {

        public override void LoadContent()
        {
            ResourceManager.Add<BitmapResource>("water", "Graphics/water.png");
            ResourceManager.Add<BitmapResource>("plains", "Graphics/plains.png");
            ResourceManager.Add<BitmapResource>("mountain", "Graphics/mountain.jpg");
            ResourceManager.Add<BitmapResource>("desert", "Graphics/desert.jpg");
            ResourceManager.Add<BitmapResource>("forest", "Graphics/forest.jpg");
        }

        public override void Initialize()
        {
            SceneManager.BeginScene(new MenuScene(false));
        }

        public override void Update(float pDeltaTime)
        {
            base.Update(pDeltaTime); //Update GameObjects
        }
    }
}
